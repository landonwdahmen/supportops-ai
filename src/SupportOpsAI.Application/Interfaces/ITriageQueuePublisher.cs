using SupportOpsAI.Application.Messaging;

namespace SupportOpsAI.Application.Interfaces;

public interface ITriageQueuePublisher
{
    Task PublishAsync(TriageJobMessage message, CancellationToken cancellationToken = default);
}
