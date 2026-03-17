namespace Iteracode.Api.Extensions;

public static class AuthorizationExtensions
{
    public const string AdminPolicy = "AdminOnly";

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminPolicy, policy =>
                policy.RequireRole("Admin"));
        });

        return services;
    }
}