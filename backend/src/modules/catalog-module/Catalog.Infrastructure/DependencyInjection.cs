using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure.Persistence;
using Catalog.Domain.Repositories;
using Catalog.Domain.Services;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Services;
using Shared.Application.Data;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<Shared.Infrastructure.Data.Interceptors.ConvertDomainEventsToOutboxMessagesInterceptor>();
            
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        // Repositórios de Domínio
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IStockReservationRepository, StockReservationRepository>();
        services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork<CatalogDbContext>>();
        services.AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();

        // Serviços de Infraestrutura
        services.AddScoped<IImageStorageService, LocalImageStorageService>();
        services.AddScoped<ISlugGenerator, SlugGenerator>();
        services.AddScoped<IStockService, StockService>();

        return services;
    }
}