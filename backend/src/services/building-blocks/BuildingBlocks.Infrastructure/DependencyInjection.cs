
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Data;
using BuildingBlocks.Application.Events;
using BuildingBlocks.Infrastructure.BackgroundJobs;
using BuildingBlocks.Infrastructure.BackgroundJobs;
using BuildingBlocks.Infrastructure.Cache;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Events.Outbox;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Infrastructure;

/// <summary>
/// Extensões para configuração de injeção de dependências da camada de infraestrutura.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços de infraestrutura base.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        // Event Bus
        // Event Bus - Outbox Pattern
        services.AddScoped<IEventBus, OutboxEventBus>();

        // Outbox
        services.Configure<OutboxConfiguration>(configuration.GetSection(OutboxConfiguration.SectionName));
        services.AddScoped<OutboxProcessor>();

        // Cache - padrão é in-memory, pode ser sobrescrito
        services.AddMemoryCache();
        services.AddScoped<ICacheService, InMemoryCacheService>();

        // Services
        services.AddScoped<IEmailService, Services.DummyEmailService>();

        return services;
    }

    /// <summary>
    /// Adiciona o processador de outbox como job em background.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddOutboxProcessorJob(this IServiceCollection services)
    {
        services.AddHostedService<OutboxProcessorJob>();
        return services;
    }

    /// <summary>
    /// Adiciona os jobs em background padrão.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundJobRunner>();

        // Jobs padrão
        // services.AddScoped<IBackgroundJob, ExpireReservationsJob>();
        // services.AddScoped<IBackgroundJob, ExpireCouponsJob>();
        // services.AddScoped<IBackgroundJob, AbandonCartsJob>();
        // services.AddScoped<IBackgroundJob, RefreshMaterializedViewsJob>();

        return services;
    }

    /// <summary>
    /// Adiciona cache distribuído (Redis).
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
            services.AddScoped<ICacheService, DistributedCacheService>();
        }

        return services;
    }

    /// <summary>
    /// Adiciona repositório e unit of work para um DbContext.
    /// </summary>
    /// <typeparam name="TContext">Tipo do DbContext.</typeparam>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddRepositories<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

/// <summary>
/// Job em background para processar o outbox.
/// </summary>
internal sealed class OutboxProcessorJob : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OutboxConfiguration _config;
    private readonly Microsoft.Extensions.Logging.ILogger<OutboxProcessorJob> _logger;

    public OutboxProcessorJob(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Options.IOptions<OutboxConfiguration> config,
        Microsoft.Extensions.Logging.ILogger<OutboxProcessorJob> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.Enabled)
        {
            _logger.LogInformation("OutboxProcessorJob desabilitado");
            return;
        }

        _logger.LogInformation(
            "OutboxProcessorJob iniciado com intervalo de {Interval}",
            _config.PollingInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();
                await processor.ProcessAsync(_config.BatchSize, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no OutboxProcessorJob");
            }

            await Task.Delay(_config.PollingInterval, stoppingToken);
        }
    }
}
