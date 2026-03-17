using Iteracode.Api.Abstractions;
using Iteracode.Api.Options;
using Iteracode.Api.Services;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Extensions;

public static class Judge0Extensions
{
    public static IServiceCollection AddJudge0(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<Judge0Options>(configuration.GetSection("Judge0"));

        services.AddHttpClient<IJudge0Client, Judge0Client>((provider, client) =>
        {
            var options = provider
                .GetRequiredService<IOptions<Judge0Options>>().Value;

            client.BaseAddress = new Uri(options.BaseUrl);

            if (!string.IsNullOrEmpty(options.AuthToken))
                client.DefaultRequestHeaders.Add("X-Auth-Token", options.AuthToken);
        });

        return services;
    }
}