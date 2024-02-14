using FluentValidation;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups.Models.Validators;

public class GetGroupRequestValidator : AbstractValidator<GetGroupRequest>
{
    public GetGroupRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
