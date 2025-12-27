using FluentValidation;
using Temp.Services.Auth.Models.Commands;

namespace Temp.Services.Auth.Models.Validators;

public class RegisterAppUserRequestValidator : AbstractValidator<RegisterAppUserRequest>
{
    public RegisterAppUserRequestValidator() {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("{EmployeeId} is required")
            .NotNull();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("{Display Name} is required")
            .NotNull()
            .MinimumLength(8)
            .WithMessage("{Display Name} is too shorter than 8 characters")
            .MaximumLength(30)
            .WithMessage("{Password} must not exceed 30 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("{Role} is required")
            .NotNull();

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("{Email} is required")
            .NotNull()
            .EmailAddress()
            .WithMessage("{Email} that is provided is not valid");

        RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("{Password} is required")
            .NotNull()
            .Matches("^(?=[^A-Z]*[A-Z])(?=[^a-z]*[a-z])(?=\\D*\\d).{8,}$")
            .WithMessage("{Password} is not complex enough");
    }
}