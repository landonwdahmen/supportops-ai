using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Application.Messaging;

namespace SupportOpsAI.Infrastructure.Messaging;

public class RabbitMqTriageJobConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqTriageJobConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(settings.TriageQueueName, durable: true, exclusive: false, autoDelete: false);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, args) =>
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Body.ToArray());
                    var message = JsonSerializer.Deserialize<TriageJobMessage>(json)
                        ?? throw new InvalidOperationException("Triage message could not be deserialized.");

                    using var scope = scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<ITriageJobProcessor>();
                    await processor.ProcessAsync(message, stoppingToken);
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Failed to consume triage job message.");
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                }
            }, stoppingToken);
        };

        channel.BasicConsume(settings.TriageQueueName, autoAck: false, consumer);
        logger.LogInformation("Listening for triage jobs on RabbitMQ queue {QueueName}.", settings.TriageQueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
