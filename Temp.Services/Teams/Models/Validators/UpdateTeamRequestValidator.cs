using FluentValidation;
using Temp.Services.Teams.Models.Commands;

namespace Temp.Services.Teams.Models.Validators;

public class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
{
    public UpdateTeamRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage("{GroupId} is required")
            .NotNull();
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{Name} is required")
            .NotNull()
            .MaximumLength(90)
            .WithMessage("{Name} must not exceed 90 characters");
    }
}
