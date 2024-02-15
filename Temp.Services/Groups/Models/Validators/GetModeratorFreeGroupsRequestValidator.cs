using FluentValidation;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups.Models.Validators;

public class GetModeratorFreeGroupsRequestValidator : AbstractValidator<GetModeratorFreeGroupsRequest>
{
    public GetModeratorFreeGroupsRequestValidator() {
        RuleFor(x => x.ModeratorId)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
