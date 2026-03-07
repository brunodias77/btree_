using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Abstractions;
using Shared.Application.Events.Handlers;
using Shared.Infrastructure.Cache;
using Shared.Infrastructure.Events;
using Shared.Infrastructure.Services;
using Shared.Infrastructure.Data.Interceptors;

namespace Shared.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços da camada de infraestrutura compartilhada.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Event Dispatcher
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        // Email (dummy para desenvolvimento)
        services.AddScoped<IEmailService, DummyEmailService>();

        // Cache: in-memory como padrão
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        // Interceptors
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        return services;
    }
}