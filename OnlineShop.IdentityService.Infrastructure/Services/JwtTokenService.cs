using Microsoft.Extensions.Options;
using OnlineShop.IdentityService.Application.Common.Interfaces;
using OnlineShop.IdentityService.Application.Common.Settings;
using OnlineShop.IdentityService.Domain.Entities;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace OnlineShop.IdentityService.Infrastructure.Services;

public class JwtTokenService(
    IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;
    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("userId", user.Id.ToString()),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenLifetimeMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        //Guid — 128 бит, но не криптостойкий.
        //RandomNumberGenerator — криптостойкий, генерирует 512 бит (64 байта).
        //Refresh token — не должен быть угадываемым.
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string rawToken)
    {
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        //для высокоэнтропийных токенов (512 случайных бит) достаточно SHA256.
        //bcrypt/Argon2 нужен для паролей (низкая энтропия).
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}