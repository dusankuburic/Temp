using FluentValidation;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups.Models.Validators;

public class GetGroupInnerTeamsRequestValidator : AbstractValidator<GetGroupInnerTeamsRequest>
{
    public GetGroupInnerTeamsRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
