using MediatR;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;