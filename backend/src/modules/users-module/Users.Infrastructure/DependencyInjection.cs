using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Outbox;
using Users.Infrastructure.Data.Context;
using Users.Infrastructure.Data.Outbox;
using Users.Domain.Repositories;
using Users.Infrastructure.Data.Repositories;
using Users.Application.Services;
using Users.Infrastructure.Services;
using Shared.Application.Data;
using Microsoft.AspNetCore.Identity;
using Users.Domain.Identity;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<Shared.Infrastructure.Data.Interceptors.ConvertDomainEventsToOutboxMessagesInterceptor>();
            
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        // Configuração do Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<UsersDbContext>()
        .AddDefaultTokenProviders();

        // Registros Locais do Outbox (específicos deste módulo)
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IInboxRepository, InboxRepository>();

        // Repositórios de Domínio
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork<UsersDbContext>>();

        // Serviços de Aplicação/Infra
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
