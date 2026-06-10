using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PeopleOfDarkMind.Application.Interfaces;
using PeopleOfDarkMind.Infrastructure.Identity;
using PeopleOfDarkMind.Infrastructure.Persistence;
using PeopleOfDarkMind.Infrastructure.Repositories;
using PeopleOfDarkMind.Application.Services;
using PeopleOfDarkMind.Infrastructure.Services;

namespace PeopleOfDarkMind.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<AppDbContext>(options =>
        {
            if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                options.UseSqlite(connectionString);
            else
                options.UseNpgsql(connectionString);
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtKey = configuration["Jwt:Key"]!;
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameEngineService, GameEngineService>();
        services.AddScoped<IGameCharacterRepository, GameCharacterRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<INpcRepository, NpcRepository>();
        services.AddScoped<IGameEventRepository, GameEventRepository>();
        services.AddScoped<IEvidenceRepository, EvidenceRepository>();
        services.AddScoped<IGameSaveRepository, GameSaveRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
