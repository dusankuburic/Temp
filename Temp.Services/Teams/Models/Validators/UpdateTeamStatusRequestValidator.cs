using FluentValidation;
using Temp.Services.Teams.Models.Commands;

namespace Temp.Services.Teams.Models.Validators;

public class UpdateTeamStatusRequestValidator : AbstractValidator<UpdateTeamStatusRequest>
{
    public UpdateTeamStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}