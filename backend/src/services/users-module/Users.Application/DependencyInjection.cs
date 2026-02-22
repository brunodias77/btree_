using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registra MediatR
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        // Registra Validadores FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // TODO: Registrar serviços de negócio se existirem implementações concretas nesta camada (o que geralmente ocorre na Infrastructure)
        // Interfaces como IIdentityService são implementadas na Infrastructure.

        return services;
    }
    
}