using MediatR;
using OnlineShop.IdentityService.Application.Features.Auth.Commands.Register;

namespace OnlineShop.IdentityService.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;