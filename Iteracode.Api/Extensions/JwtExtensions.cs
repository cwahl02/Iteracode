using Iteracode.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Iteracode.Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services
    )
    {
        AddJwtConfiguration(services, services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        return services;    
    }

    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
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
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    Console.WriteLine("OnMessageReceived called");
                    // Allow access token from cookie as fallback if needed
                    ctx.Token = ctx.Request.Headers.Authorization
                        .FirstOrDefault()?.Split(" ").Last();
                    return Task.CompletedTask;
                },
                OnTokenValidated = ctx =>
                {
                    var jti = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    

                    return Task.CompletedTask;
                },
            };
        });
        
        return services;
    }
}