using MediatR;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string? FirstName = null,
    string? LastName = null) : IRequest<AuthResponse>;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    IList<string> Roles);