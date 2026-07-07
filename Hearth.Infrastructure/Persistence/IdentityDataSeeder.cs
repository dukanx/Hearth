using Hearth.Application.Common;
using Microsoft.AspNetCore.Identity;

namespace Hearth.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = { Roles.Adult, Roles.Child };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}
