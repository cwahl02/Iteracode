using Iteracode.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Iteracode.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseExtensions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}