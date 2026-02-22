using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Carts.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCartsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}