using FluentValidation;
using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Employees.Models.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator() {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("{TeamId} is required")
            .NotNull();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("{FirstName} is required")
            .NotNull()
            .MaximumLength(30)
            .WithMessage("{FirstName} must not exceed 30 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("{LastName} is required")
            .NotNull()
            .MaximumLength(30)
            .WithMessage("{LastName} must not exceed 30 characters");
    }
}