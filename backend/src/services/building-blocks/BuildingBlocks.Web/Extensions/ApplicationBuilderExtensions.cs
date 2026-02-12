using BuildingBlocks.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Extensões para IApplicationBuilder.
/// Configuração de middlewares e pipeline da aplicação.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adiciona middlewares padrão do BuildingBlocks.
    /// Deve ser chamado no início do pipeline.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Application builder.</returns>
    public static IApplicationBuilder UseBuildingBlocksMiddlewares(this IApplicationBuilder app)
    {
        // Correlation ID deve ser primeiro para rastreabilidade
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Exception handling deve vir logo após para capturar erros
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Request logging para monitoramento
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }

    /// <summary>
    /// Configura Swagger UI.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <param name="routePrefix">Prefixo da rota (padrão: swagger).</param>
    /// <returns>Application builder.</returns>
    public static IApplicationBuilder UseSwaggerWithUI(
        this IApplicationBuilder app,
        string routePrefix = "swagger")
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = $"{routePrefix}/{{documentName}}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/{routePrefix}/v1/swagger.json", "BCommerce API v1");
            options.RoutePrefix = routePrefix;

            // Configurações de UI
            options.DocumentTitle = "BCommerce API";
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
        });

        return app;
    }

    /// <summary>
    /// Configura CORS para desenvolvimento.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <param name="policyName">Nome da policy (padrão: AllowAll).</param>
    /// <returns>Application builder.</returns>
    public static IApplicationBuilder UseDevelopmentCors(
        this IApplicationBuilder app,
        string policyName = "AllowAll")
    {
        app.UseCors(policyName);
        return app;
    }

    /// <summary>
    /// Configura endpoints de health check.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Application builder.</returns>
    public static IApplicationBuilder UseHealthChecksEndpoints(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
        app.UseHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        app.UseHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        return app;
    }
}
