using Hearth.Application.Common.Interfaces;
using Hearth.Infrastructure.Identity;
using Hearth.Infrastructure.Persistence;
using Hearth.Infrastructure.Persistence.Repositories;
using Hearth.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hearth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Core Identity (token-API, bez cookie mašinerije).
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // Poravnato sa FluentValidation "min 6" — jednostavna, predvidiva pravila.
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // Repository + Unit of Work (svi dele isti scoped AppDbContext / change-tracker).
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IShoppingItemRepository, ShoppingItemRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IHouseholdRepository, HouseholdRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IJoinCodeGenerator, JoinCodeGenerator>();

        return services;
    }
}
