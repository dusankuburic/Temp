using FluentValidation;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses.Models.Validators;

public class GetEmploymentStatusRequestValidator : AbstractValidator<GetEmploymentStatusRequest>
{
    public GetEmploymentStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
