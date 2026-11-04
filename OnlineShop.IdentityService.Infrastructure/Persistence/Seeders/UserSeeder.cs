using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Infrastructure.Persistence.Seeders;

public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var seedUsers = configuration
            .GetSection("SeedData:Users")
            .Get<List<SeedUserOptions>>();

        if (seedUsers is null || seedUsers.Count == 0)
            return;

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(UserSeeder));

        foreach (var seed in seedUsers)
        {
            if (await userManager.FindByEmailAsync(seed.Email) is not null)
                continue;

            var user = new ApplicationUser
            {
                UserName = seed.Email,
                Email = seed.Email,
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, seed.Password);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create user {Email}: {Errors}",
                    seed.Email,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                continue;
            }

            var roleResult = await userManager.AddToRoleAsync(user, seed.Role);
            if(roleResult.Succeeded)
            {
                logger.LogInformation("User {Email} created with role {Role}", seed.Email, seed.Role);
            }
            else
            {
                logger.LogError("Failed to add role {Role} to user {Email}: {Errors}",
                   seed.Role, seed.Email,
                   string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}