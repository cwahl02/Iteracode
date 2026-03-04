namespace Iteracode.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseExtensions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Uncomment and configure when ApplicationDbContext and connection string are ready
        // services.AddDbContext<>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}