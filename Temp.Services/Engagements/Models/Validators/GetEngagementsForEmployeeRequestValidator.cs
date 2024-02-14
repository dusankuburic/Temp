using FluentValidation;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements.Models.Validators;

public class GetEngagementsForEmployeeRequestValidator : AbstractValidator<GetEngagementsForEmployeeRequest>
{
    public GetEngagementsForEmployeeRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
