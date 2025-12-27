using FluentValidation;
using Temp.Services.Auth.Models.Commands;

namespace Temp.Services.Auth.Models.Validators;

public class LoginAppUserRequestValidator : AbstractValidator<LoginAppUserRequest>
{
    public LoginAppUserRequestValidator() {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("{Username} is required")
            .NotNull();

        RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("{Password} is required")
            .NotNull();
    }
}