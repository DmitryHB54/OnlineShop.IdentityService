using OnlineShop.IdentityService.WebApi.ExceptionHandling;

namespace OnlineShop.IdentityService.WebApi.Extensions;

public static class GlobalExceptionExtensions
{
    public static IServiceCollection AddGlobalExtensionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}