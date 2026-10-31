
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Application.Common.Interfaces;
public interface ITokenService
{
    /// <summary>Генерирует JWT access token.</summary>
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    /// <summary>Генерирует случайный refresh token (raw string).</summary>
    string GenerateRefreshToken();

    /// <summary>Хеширует refresh token для хранения в БД.</summary>
    string HashToken(string rawToken);
}