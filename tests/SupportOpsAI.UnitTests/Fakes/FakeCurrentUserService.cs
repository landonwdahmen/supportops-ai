using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.UnitTests.Fakes;

public class FakeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
    public bool IsAuthenticated => UserId.HasValue;
}
