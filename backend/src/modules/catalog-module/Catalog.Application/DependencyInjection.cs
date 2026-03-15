using Microsoft.Extensions.DependencyInjection;
using Shared.Application;
using System.Reflection;
using FluentValidation;
using Catalog.Application.Features.Categories.GetById;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Scan automático de UseCases e Handlers na Catalog.Application
        services.AddApplicationServices(assembly);
        
        // Explicitly register GetCategoryByIdUseCase to fix DI resolution error
        services.AddScoped<IGetCategoryByIdUseCase, GetCategoryByIdUseCase>();
        
        // Scan automático de Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}