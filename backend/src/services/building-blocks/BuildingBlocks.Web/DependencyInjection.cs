using BuildingBlocks.Web.Filters;
using BuildingBlocks.Web.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

/// <summary>
/// Extensões para configuração de injeção de dependências da camada Web.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada Web.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        // Configuração de controllers com filtros
        services.AddControllers(options =>
        {
            // Filtro de validação automática
            options.Filters.Add<ValidationFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Desabilita validação automática do ModelState
            // (usamos nosso próprio filtro)
            options.SuppressModelStateInvalidFilter = true;
        });

        // CORS para desenvolvimento
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("Production", builder =>
            {
                builder
                    .WithOrigins("https://bcommerce.com", "https://admin.bcommerce.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // Swagger
        services.AddSwaggerConfiguration();

        // Health checks
        services.AddHealthChecks();

        return services;
    }

    /// <summary>
    /// Adiciona configuração completa de controllers com opções customizadas.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="configureController">Configuração adicional de controllers.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        Action<Microsoft.AspNetCore.Mvc.MvcOptions>? configureController)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
            configureController?.Invoke(options);
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }

    /// <summary>
    /// Adiciona health check personalizado.
    /// </summary>
    /// <typeparam name="THealthCheck">Tipo do health check.</typeparam>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="name">Nome do health check.</param>
    /// <param name="tags">Tags do health check.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddCustomHealthCheck<THealthCheck>(
        this IServiceCollection services,
        string name,
        params string[] tags)
        where THealthCheck : class, Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        services.AddHealthChecks()
            .AddCheck<THealthCheck>(name, tags: tags);

        return services;
    }
}
