using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tickey.Sevices;

namespace Tickey.Middleware;

public static class DependencyInjection 
{
    public static IServiceCollection AddErrorHandlingAndLogs(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddLogging();

        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddNpgSql(configuration.GetConnectionString("TickeyDB")!);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TickeyDB")));

        return services;
    }
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true,
        });

        services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                .AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("Fixed", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromSeconds(30);
            });
        });

        return services;

    }

    public static IServiceCollection DependentServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<TokenService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
