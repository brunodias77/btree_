using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Identity;
using BuildingBlocks.Application.Data;
using Users.Application.Repositories;
using Users.Application.Services;
using Users.Infrastructure.Data.Context;
using Users.Infrastructure.Data.Repositories;
using Users.Infrastructure.Data.UnitOfWork;
using Users.Infrastructure.Services;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuração do DbContext com PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            
            // Configura interceptors (Auditing, DomainEvents, SoftDelete)
            options.AddInterceptors(
                sp.GetRequiredService<AuditInterceptor>(),
                sp.GetRequiredService<DomainEventInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>());
        });

        // Registra o DbContext como serviço escopado
        services.AddScoped<UsersDbContext>();
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<UsersDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUnitOfWork, UsersUnitOfWork>();

        return services;
    }
}