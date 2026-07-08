using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Hearth.Api;
using Hearth.Api.Common;
using Hearth.Api.Hubs;
using Hearth.Application;
using Hearth.Application.Common.Interfaces;
using Hearth.Infrastructure;
using Hearth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Railway (i slični hostovi): PORT env promenljiva određuje gde slušamo.
if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } port)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Railway Postgres daje DATABASE_URL kao URI — konvertuj u Npgsql format
// ako eksplicitan connection string nije postavljen.
if (builder.Configuration.GetConnectionString("DefaultConnection") is null
    && Environment.GetEnvironmentVariable("DATABASE_URL") is { Length: > 0 } databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    builder.Configuration["ConnectionStrings:DefaultConnection"] =
        $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={Uri.UnescapeDataString(userInfo[0])};" +
        $"Password={Uri.UnescapeDataString(userInfo[1])};" +
        "Ssl Mode=Require";
}

// --- Services ---

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

// Sloj-po-sloj DI: MediatR/validatori (Application) i DbContext/Identity/servisi (Infrastructure).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Trenutni korisnik iz JWT-a (za zaštićene endpointe).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Realtime obaveštenja (SignalR). Naš userId je u 'sub' -> custom IUserIdProvider.
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<IRealtimeNotifier, RealtimeNotifier>();

// JWT autentifikacija — JWT je default scheme (nema cookie da otme default).
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Jwt:Key nije postavljen (postavi ga u user-secrets).");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Bez remapiranja claim-ova na dugačke XML URI-jeve — 'sub' ostaje 'sub'.
        options.MapInboundClaims = false;

        // WebSocket ne šalje Authorization header — SignalR klijent nosi token u ?access_token=.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                    context.Token = accessToken;

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            // userId čitamo iz 'sub', a [Authorize(Roles=...)] iz standardnog role claim-a.
            NameClaimType = "sub",
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- HTTP pipeline ---

// Iza reverse proxy-ja (Railway): poštuj X-Forwarded-Proto/For da https radi ispravno.
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
};
forwardedOptions.KnownIPNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// U produkciji API servira i frontend (React build u wwwroot).
var serveSpa = File.Exists(Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html"));
if (serveSpa)
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// SPA fallback: nepoznate rute (npr. /tasks posle refresh-a) idu na index.html.
if (serveSpa)
    app.MapFallbackToFile("index.html");

// Migracije + seed Identity uloga (Adult/Child) pri pokretanju.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await IdentityDataSeeder.SeedRolesAsync(roleManager);
}

app.Run();