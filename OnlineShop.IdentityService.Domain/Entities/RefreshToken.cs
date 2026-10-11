namespace OnlineShop.IdentityService.Domain.Entities;
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    ///<summary>Хеш токена (не сам токен!).</summary>
    public string TokenHash { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }

    ///<summary>Id токена, которым заменен текущий (дла rotation)</summary>
    public Guid? ReplacedByTokenId { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpireAt;

}