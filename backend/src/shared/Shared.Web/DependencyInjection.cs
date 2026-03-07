using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Web.Filters;
using Shared.Web.Logging;
using Shared.Web.Swagger;

namespace Shared.Web;

public static class DependencyInjection
{
    /// <summary>
    /// Registra todos os serviços da camada web compartilhada:
    /// Serilog, Swagger, ValidationFilter, IdempotencyFilter, e configurações de API.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuração da aplicação (para Serilog).</param>
    /// <param name="appName">Nome da aplicação (ex: "BCommerce.API").</param>
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string appName = "BCommerce.API")
    {
        // ── Logging (Serilog) ──
        services.AddSerilogLogging(configuration, appName);

        // ── Swagger / OpenAPI ──
        services.AddSwaggerConfiguration();

        // ── Controllers ──
        services.AddControllers(options =>
        {
            // Adiciona ValidationFilter globalmente em todos os controllers
            options.Filters.Add<ValidationFilter>();
        });

        // Desabilita o filtro automático de ModelState do ASP.NET Core
        // para usar o ValidationFilter customizado
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // ── Filters como serviços (para uso com [ServiceFilter]) ──
        services.AddScoped<IdempotencyFilter>();

        // ── Cache em memória (usado pelo IdempotencyFilter) ──
        services.AddMemoryCache();

        return services;
    }
}