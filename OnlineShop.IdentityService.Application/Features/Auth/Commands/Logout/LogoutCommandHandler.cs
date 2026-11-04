using MediatR;
using OnlineShop.IdentityService.Application.Common.Interfaces;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeAsync(request.RefreshToken, "Logout", cancellationToken);
    }
}