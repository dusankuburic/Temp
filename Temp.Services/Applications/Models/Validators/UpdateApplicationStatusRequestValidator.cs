using FluentValidation;
using Temp.Services.Applications.Models.Commands;

namespace Temp.Services.Applications.Models.Validators;

public class UpdateApplicationStatusRequestValidator : AbstractValidator<UpdateApplicationStatusRequest>
{
    public UpdateApplicationStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();

        RuleFor(x => x.ModeratorId)
            .NotEmpty()
            .WithMessage("{ModeratorId} is required")
            .NotNull();
    }
}
