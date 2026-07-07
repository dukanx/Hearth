using System.Security.Claims;
using Hearth.Application.Common.Interfaces;

namespace Hearth.Api.Common;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid? Id
    {
        get
        {
            var principal = _accessor.HttpContext?.User;
            var value = principal?.FindFirstValue("sub")
                        ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? HouseholdId
    {
        get
        {
            var value = _accessor.HttpContext?.User.FindFirstValue("householdId");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Role
        => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated
        => _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
