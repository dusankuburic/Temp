using FluentValidation;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams.Models.Validators;

public class GetUserTeamRequestValidator : AbstractValidator<GetUserTeamRequest>
{
    public GetUserTeamRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
