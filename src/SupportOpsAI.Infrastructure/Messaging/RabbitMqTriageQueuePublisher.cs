using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Application.Messaging;

namespace SupportOpsAI.Infrastructure.Messaging;

public class RabbitMqTriageQueuePublisher(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqTriageQueuePublisher> logger) : ITriageQueuePublisher
{
    public Task PublishAsync(TriageJobMessage message, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            DispatchConsumersAsync = false
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(settings.TriageQueueName, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.CorrelationId = message.CorrelationId;
        properties.ContentType = "application/json";

        channel.BasicPublish(exchange: string.Empty, routingKey: settings.TriageQueueName, basicProperties: properties, body: body);
        logger.LogInformation("Published triage job {TriageJobId} for ticket {TicketId}.", message.TriageJobId, message.TicketId);

        return Task.CompletedTask;
    }
}
