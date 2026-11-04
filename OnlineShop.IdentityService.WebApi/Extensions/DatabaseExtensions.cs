using Microsoft.EntityFrameworkCore;
using OnlineShop.IdentityService.Infrastructure.Persistence;

namespace OnlineShop.IdentityService.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using(var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}