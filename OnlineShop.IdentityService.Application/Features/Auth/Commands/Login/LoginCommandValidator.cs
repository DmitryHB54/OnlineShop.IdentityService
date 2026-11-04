using FluentValidation;

namespace OnlineShop.IdentityService.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Incorrect email format");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}