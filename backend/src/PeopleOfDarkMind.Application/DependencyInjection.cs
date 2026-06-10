using Microsoft.Extensions.DependencyInjection;
using PeopleOfDarkMind.Application.Services;

namespace PeopleOfDarkMind.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}
