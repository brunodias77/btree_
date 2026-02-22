using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Application;

/// <summary>
/// Extensões para configuração de injeção de dependências da camada de aplicação.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada de aplicação.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="assemblies">Assemblies para scan de handlers e validators.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        // Registra MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);

            // Adiciona behaviors do pipeline
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Registra validadores FluentValidation
        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

        return services;
    }

    /// <summary>
    /// Adiciona os serviços da camada de aplicação com assembly do tipo especificado.
    /// </summary>
    /// <typeparam name="T">Tipo de referência para obter assembly.</typeparam>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddApplicationServices<T>(this IServiceCollection services)
    {
        return services.AddApplicationServices(typeof(T).Assembly);
    }

    /// <summary>
    /// Adiciona comportamento de idempotência ao pipeline MediatR.
    /// Deve ser chamado após AddApplicationServices.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddIdempotencyBehavior(this IServiceCollection services)
    {
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(Idempotency.IdempotencyBehavior<,>));

        return services;
    }
}
