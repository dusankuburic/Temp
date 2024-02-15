using FluentValidation;
using Temp.Services.EmploymentStatuses.Models.Commands;

namespace Temp.Services.EmploymentStatuses.Models.Validators;

public class UpdateEmploymentStatusRequestValidator : AbstractValidator<UpdateEmploymentStatusRequest>
{
    public UpdateEmploymentStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{Name} is required")
            .NotNull()
            .MaximumLength(90)
            .WithMessage("{Name} must not exceed 90 characters");
    }
}
