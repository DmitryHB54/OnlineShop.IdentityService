using MassTransit;

namespace OnlineShop.IdentityService.WebApi.Extensions;

public static class WebServicesExctensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });

        return services;
    }
}