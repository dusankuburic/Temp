using FluentValidation;
using Temp.Services.Engagements.Models.Commands;

namespace Temp.Services.Engagements.Models.Validators;

public class CreateEngagementRequestValidator : AbstractValidator<CreateEngagementRequest>
{
    public CreateEngagementRequestValidator() {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("{EmployeeId} is required")
            .NotNull();

        RuleFor(x => x.WorkplaceId)
            .NotEmpty()
            .WithMessage("{WorkplaceId} is required")
            .NotNull();

        RuleFor(x => x.EmploymentStatusId)
            .NotEmpty()
            .WithMessage("{EmploymentStatusId} is required")
            .NotNull();

        RuleFor(x => x.DateFrom)
            .NotEmpty()
            .WithMessage("{DateFrom} is required")
            .NotNull();

        RuleFor(x => x.DateTo)
            .NotEmpty()
            .WithMessage("{DateTo} is required")
            .NotNull();

        RuleFor(x => x.Salary)
            .NotEmpty()
            .WithMessage("{Salary} is required")
            .NotNull();
    }
}
