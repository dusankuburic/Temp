using FluentValidation;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements.Models.Validators;

public class GetUserEmployeeEngagementsRequestValidator : AbstractValidator<GetUserEmployeeEngagementsRequest>
{
    public GetUserEmployeeEngagementsRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
