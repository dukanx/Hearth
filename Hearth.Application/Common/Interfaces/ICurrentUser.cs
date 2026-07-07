namespace Hearth.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get; }
    Guid? HouseholdId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
