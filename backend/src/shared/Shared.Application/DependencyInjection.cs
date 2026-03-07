using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Application.Events.Handlers;
using Shared.Application.UseCases;
using Shared.Domain.Events;
using System.Reflection;

namespace Shared.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada de aplicação.
    /// Faz scan automático de handlers de eventos e use cases nos assemblies informados.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="assemblies">Assemblies para scan de handlers e use cases.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = [Assembly.GetCallingAssembly()];

        services.AddEventHandlers(assemblies);
        services.AddUseCases(assemblies);

        return services;
    }

    /// <summary>
    /// Registra automaticamente todos os IEventHandler&lt;TEvent&gt; encontrados nos assemblies.
    /// Inclui IDomainEventHandler e IIntegrationEventHandler (que herdam de IEventHandler).
    /// Registra como Scoped para funcionar com DbContext e Unit of Work.
    /// </summary>
    private static void AddEventHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        var handlerInterfaceType = typeof(IEventHandler<>);

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType));

            foreach (var handlerType in handlerTypes)
            {
                var implementedHandlerInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType);

                foreach (var handlerInterface in implementedHandlerInterfaces)
                {
                    services.AddScoped(handlerInterface, handlerType);
                }
            }
        }
    }

    /// <summary>
    /// Registra automaticamente todos os IUseCase&lt;TInput, TOutput&gt; encontrados nos assemblies.
    /// Registra como Scoped.
    /// </summary>
    private static void AddUseCases(this IServiceCollection services, Assembly[] assemblies)
    {
        var useCaseInterfaceType = typeof(IUseCase<,>);

        foreach (var assembly in assemblies)
        {
            var useCaseTypes = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == useCaseInterfaceType));

            foreach (var useCaseType in useCaseTypes)
            {
                // Registra contra IUseCase<TInput, TOutput>
                var implementedInterfaces = useCaseType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == useCaseInterfaceType);

                foreach (var useCaseInterface in implementedInterfaces)
                {
                    services.AddScoped(useCaseInterface, useCaseType);
                }

                // Registra interfaces específicas (ex: IRegisterUserUseCase) que estendem IUseCase<,>
                var specificInterfaces = useCaseType.GetInterfaces()
                    .Where(i => i.GetInterfaces().Any(gi =>
                        gi.IsGenericType && gi.GetGenericTypeDefinition() == useCaseInterfaceType));

                foreach (var specificInterface in specificInterfaces)
                {
                    services.AddScoped(specificInterface, useCaseType);
                }
            }
        }
    }
}
