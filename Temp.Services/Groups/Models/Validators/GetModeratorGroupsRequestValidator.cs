using FluentValidation;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups.Models.Validators;

public class GetModeratorGroupsRequestValidator : AbstractValidator<GetModeratorGroupsRequest>
{
    public GetModeratorGroupsRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
