namespace OnlineShop.IdentityService.Application.Common.Interfaces;
public interface IRefreshTokenService
{
    /// <summary>Создаёт новый refresh token, сохраняет хеш в БД, возвращает raw токен.</summary>
    Task<string> CreateAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Проверяет raw токен. Возвращает userId, если валиден.</summary>
    Task<Guid?> ValidationAsync(string rawToken, CancellationToken ct = default);
    
    /// <summary>Отзывает токен (logout).</summary>
    Task RevokeAsync(string rawToken, string reason, CancellationToken ct = default);

    /// <summary>Ротирует: отзывает старый, создаёт новый.</summary>
    Task<string> RotateAsync(string rawToken, CancellationToken ct = default);
}