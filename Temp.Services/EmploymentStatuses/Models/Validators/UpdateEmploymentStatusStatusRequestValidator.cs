using FluentValidation;
using Temp.Services.EmploymentStatuses.Models.Commands;

namespace Temp.Services.EmploymentStatuses.Models.Validators;

public class UpdateEmploymentStatusStatusRequestValidator : AbstractValidator<UpdateEmploymentStatusStatusRequest>
{
    public UpdateEmploymentStatusStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}