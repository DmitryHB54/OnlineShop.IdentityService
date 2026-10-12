using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.IdentityService.Application.Common.Behaviors;
using System.Reflection;

namespace OnlineShop.IdentityService.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicaton(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(cfg => { }, assembly);

        return services;
    }
}