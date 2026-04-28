using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Worker.Services;

public class WorkerCurrentUserService : ICurrentUserService
{
    public Guid? UserId => null;
    public string? Email => null;
    public UserRole? Role => null;
    public bool IsAuthenticated => false;
}
