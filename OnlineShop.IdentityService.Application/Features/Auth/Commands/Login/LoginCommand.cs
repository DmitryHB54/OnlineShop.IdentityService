using MediatR;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;