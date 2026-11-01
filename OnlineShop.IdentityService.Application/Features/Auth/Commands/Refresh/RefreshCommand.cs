using MediatR;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<AuthResponse>;