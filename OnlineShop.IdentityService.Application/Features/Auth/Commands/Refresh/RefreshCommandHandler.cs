using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineShop.IdentityService.Application.Common.Interfaces;
using OnlineShop.IdentityService.Application.Common.Exceptions;
using OnlineShop.IdentityService.Application.Common.Settings;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtSettings _jwtSettings;

    public RefreshCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenService.ValidationAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            throw new UnauthorizedException("Invalid or expired refresh token");

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user is null)
            throw new NotFoundException("User not found");
        if(!user.IsActive)
            throw new UnauthorizedException("Account is deactivated");

        var newRefreshToken = await _refreshTokenService.RotateAsync(request.RefreshToken, cancellationToken);
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes),
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles));

        throw new NotImplementedException();
    }
}