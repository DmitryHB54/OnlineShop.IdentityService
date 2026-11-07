using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OnlineShop.IdentityService.Application.Common.Interfaces;
using OnlineShop.Shared.Contracts;
using OnlineShop.IdentityService.Domain.Constants;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;
public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IPublishEndpoint publishEndpoint,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("User with this email already exists");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if(!createResult.Succeeded)
        {
            var errors = string.Join(": ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = await refreshTokenService.CreateAsync(user.Id, cancellationToken);

        logger.LogInformation("Publishing UserRegistered for {Email}", user.Email);
        await publishEndpoint.Publish(new UserRegistered(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAt));
        logger.LogInformation("UserRegistered published");

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles));
    }
}