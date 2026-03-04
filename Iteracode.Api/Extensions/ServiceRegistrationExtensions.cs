using Iteracode.Api.InjectionMarkers;
using System.Reflection;

namespace Iteracode.Api.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddInjectionMarkers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            if (typeof(IScopedService).IsAssignableFrom(type))
                Register(services, type, ServiceLifetime.Scoped);

            if (typeof(ITransientService).IsAssignableFrom(type))
                Register(services, type, ServiceLifetime.Transient);

            if (typeof(ISingletonService).IsAssignableFrom(type))
                Register(services, type, ServiceLifetime.Singleton);
        }

        return services;
    }

    private static void Register(IServiceCollection services, Type implementation, ServiceLifetime lifetime)
    {
        var serviceInterfaces = implementation.GetInterfaces()
            .Where(i =>
                i != typeof(IScopedService) &&
                i != typeof(ITransientService) &&
                i != typeof(ISingletonService));

        foreach (var service in serviceInterfaces)
        {
            services.Add(new ServiceDescriptor(service, implementation, lifetime));
        }
    }
}