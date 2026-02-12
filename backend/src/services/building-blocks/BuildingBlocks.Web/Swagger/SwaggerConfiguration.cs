using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BuildingBlocks.Web.Swagger;

/// <summary>
/// Configuração do Swagger/OpenAPI.
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adiciona configuração do Swagger.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="title">Título da API.</param>
    /// <param name="description">Descrição da API.</param>
    /// <param name="version">Versão da API.</param>
    /// <returns>Coleção de serviços.</returns>
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services,
        string title = "BCommerce API",
        string description = "API do sistema de e-commerce modular",
        string version = "v1")
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Description = description,
                Version = version,
                Contact = new OpenApiContact
                {
                    Name = "Suporte",
                    Email = "suporte@bcommerce.com"
                }
            });

            // Configuração de autenticação JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Inclui comentários XML
            var xmlFilename = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Ordenação de endpoints
            options.OrderActionsBy(apiDesc =>
                $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

            // Customização de operação IDs
            options.CustomOperationIds(apiDesc =>
            {
                var controller = apiDesc.ActionDescriptor.RouteValues["controller"];
                var action = apiDesc.ActionDescriptor.RouteValues["action"];
                return $"{controller}_{action}";
            });
        });

        return services;
    }

    /// <summary>
    /// Adiciona documentação XML de múltiplos assemblies.
    /// </summary>
    public static IServiceCollection AddSwaggerXmlComments(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.ConfigureSwaggerGen(options =>
        {
            foreach (var assembly in assemblies)
            {
                var xmlFilename = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        });

        return services;
    }
}
