using Microsoft.AspNetCore.Identity;

namespace OnlineShop.IdentityService.Domain.Entities;
public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public ApplicationRole() { }

    public ApplicaitonRole(string roleName, string? description = null)
        : base(roleName)
    {
        Description = description;
    }
}