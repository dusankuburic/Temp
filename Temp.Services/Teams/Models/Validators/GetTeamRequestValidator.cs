using FluentValidation;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams.Models.Validators;

public class GetTeamRequestValidator : AbstractValidator<GetTeamRequest>
{
    public GetTeamRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
