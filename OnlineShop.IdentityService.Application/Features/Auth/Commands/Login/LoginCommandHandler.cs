using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineShop.IdentityService.Application.Common.Exceptions;
using OnlineShop.IdentityService.Application.Common.Interfaces;
using OnlineShop.IdentityService.Application.Common.Settings;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
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

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user is null)
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, command.Password);
        if(!passwordValid)
            throw new UnauthorizedException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = await _refreshTokenService.CreateAsync(user.Id, cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes),
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles));
    }
}