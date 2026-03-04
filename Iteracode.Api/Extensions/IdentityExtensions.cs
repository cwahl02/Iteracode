namespace Iteracode.Api.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // TODO: Move Identity options to appsettings.json and bind them to a strongly typed class
        // Uncomment when User model, ApplicationDbContext and other Identity related classes are implemented
        // services.AddIdentity<>(options =>
        // {
        //     options.Password.RequireDigit = true;
        //     options.Password.RequireLowercase = true;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequireNonAlphanumeric = false;
        //     options.Password.RequiredLength = 6;

        //     options.User.RequireUniqueEmail = true;

        //     // In future move lockout settings to appsettings.json
        //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //     options.Lockout.MaxFailedAccessAttempts = 5;
        // })
        // .AddEntityFrameworkStores<DataContext>()
        // .AddDefaultTokenProviders();

        return services;
    }
}