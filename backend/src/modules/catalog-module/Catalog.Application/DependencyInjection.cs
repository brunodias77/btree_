using Microsoft.Extensions.DependencyInjection;
using Shared.Application;
using System.Reflection;
using FluentValidation;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Scan automático de UseCases e Handlers na Catalog.Application
        services.AddApplicationServices(assembly);
        
        // Scan automático de Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}