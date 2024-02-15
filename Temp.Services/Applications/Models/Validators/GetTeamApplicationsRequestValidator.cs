using FluentValidation;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications.Models.Validators;

public class GetTeamApplicationsRequestValidator : AbstractValidator<GetTeamApplicationsRequest>
{
    public GetTeamApplicationsRequestValidator() {
        RuleFor(x => x.ModeratorId)
            .NotEmpty()
            .WithMessage("{ModeratorId} is required")
            .NotNull();

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("{TeamId} is required")
            .NotNull();
    }
}
