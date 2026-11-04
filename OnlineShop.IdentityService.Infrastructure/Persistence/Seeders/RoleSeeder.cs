using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineShop.IdentityService.Domain.Constants;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Infrastructure.Persistence.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(RoleSeeder));

        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new ApplicationRole(roleName, $"{roleName} role");
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
                logger.LogInformation("Role '{Role}' created", roleName);
            else
                logger.LogError("Failed to create role '{Role}': {Errors}",
                    roleName,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}