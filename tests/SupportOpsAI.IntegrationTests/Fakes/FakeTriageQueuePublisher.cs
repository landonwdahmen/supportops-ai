using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Application.Messaging;

namespace SupportOpsAI.IntegrationTests.Fakes;

public class FakeTriageQueuePublisher : ITriageQueuePublisher
{
    public List<TriageJobMessage> PublishedMessages { get; } = [];
    public bool ThrowOnPublish { get; set; }

    public Task PublishAsync(TriageJobMessage message, CancellationToken cancellationToken = default)
    {
        if (ThrowOnPublish)
        {
            throw new InvalidOperationException("Publish failed.");
        }

        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }
}
