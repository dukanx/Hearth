using Microsoft.AspNetCore.Identity;
using Hearth.Domain.Entities;

namespace Hearth.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid? HouseholdId { get; set; }
    public Household? Household { get; set; }
}