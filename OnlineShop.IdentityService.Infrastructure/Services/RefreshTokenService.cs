using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineShop.IdentityService.Application.Common.Interfaces;
using OnlineShop.IdentityService.Application.Common.Settings;
using OnlineShop.IdentityService.Domain.Entities;
using OnlineShop.IdentityService.Infrastructure.Persistence;

namespace OnlineShop.IdentityService.Infrastructure.Services;

public class RefreshTokenService(
    IdentityDbContext dbContext,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtOptions) : IRefreshTokenService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;
    public async Task<string> CreateAsync(Guid userId, CancellationToken ct = default)
    {
        var rawToken = tokenService.GenerateRefreshToken();
        var tokenHash = tokenService.HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenLifetimeDays)
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(ct);

        return rawToken;
    }

    public async Task<Guid?> ValidationAsync(string rawToken, CancellationToken ct = default)
    {
        var tokenHash = tokenService.HashToken(rawToken);
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct)
            ?? throw new InvalidOperationException("Refresh token not found"); ;

        if (!token.IsActive)
            throw new InvalidOperationException("Refresh token is not active");

        return token.UserId;
    }

    public async Task RevokeAsync(string rawToken, string reason, CancellationToken ct = default)
    {
        var tokenHash = tokenService.HashToken(rawToken);

        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct)
            ?? throw new InvalidOperationException("Refresh token not found"); ;

        if (token.RevokedAt is not null)
            throw new InvalidOperationException("Refresh token is already revoked");

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedReason = reason;
        await dbContext.SaveChangesAsync();
    }

    public async Task<string> RotateAsync(string rawToken, CancellationToken ct = default)
    {
        var tokenHash = tokenService.HashToken(rawToken);

        var oldToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct)
            ?? throw new InvalidOperationException("Refresh token not found");

        if (!oldToken.IsActive)
            throw new InvalidOperationException("Refresh token is not active");

        var newRawToken = tokenService.GenerateRefreshToken();
        var newTokenHash = tokenService.HashToken(newRawToken);

        var newToken = new RefreshToken
        {
            UserId = oldToken.UserId,
            TokenHash = newTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenLifetimeDays)
        };

        dbContext.RefreshTokens.Add(newToken);

        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.RevokedReason = "Rotated";
        oldToken.ReplacedByTokenId = newToken.Id;

        await dbContext.SaveChangesAsync(ct);
        
        return newRawToken;
    }
}