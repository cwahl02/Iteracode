using Iteracode.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Iteracode.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseExtensions(
        this IServiceCollection services
    )
    {
        AddDatabaseExtensions(services, services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        return services;    
    }

    public static IServiceCollection AddDatabaseExtensions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}