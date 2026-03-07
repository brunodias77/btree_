using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Outbox;

public static class DependencyInjection
{
    public static IServiceCollection AddOutBoxServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IOutboxService, OutboxService>();
        
        // Registra o processador em background para todo o sistema (cautela se rodar várias instâncias de Api/Worker)
        services.AddHostedService<ProcessOutboxMessagesBackgroundService>();

        return services;
    }
}