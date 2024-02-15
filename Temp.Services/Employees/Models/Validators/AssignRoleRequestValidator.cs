using FluentValidation;
using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Employees.Models.Validators;

public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("{Username} is required")
            .NotNull()
            .MaximumLength(30)
            .WithMessage("{Username} must not exceed 30 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("{Password} is required")
            .NotNull()
            .MaximumLength(30)
            .WithMessage("{Password} must not exceed 30 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("{Role} is required")
            .NotNull();
    }
}
