namespace OnlineShop.IdentityService.Infrastructure.Persistence.Seeders;

public class SeedUserOptions
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = null!;
}