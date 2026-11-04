using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;
using OnlineShop.IdentityService.Application.Common.Exceptions;
using OnlineShop.IdentityService.Domain.Entities;

namespace OnlineShop.IdentityService.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles);
    }
}