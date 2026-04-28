using SupportOpsAI.Application.Messaging;

namespace SupportOpsAI.Application.Interfaces;

public interface ITriageJobProcessor
{
    Task ProcessAsync(TriageJobMessage message, CancellationToken cancellationToken = default);
}
