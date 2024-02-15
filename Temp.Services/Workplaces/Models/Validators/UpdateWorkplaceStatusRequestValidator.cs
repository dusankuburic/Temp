using FluentValidation;
using Temp.Services.Workplaces.Models.Commands;

namespace Temp.Services.Workplaces.Models.Validators;

public class UpdateWorkplaceStatusRequestValidator : AbstractValidator<UpdateWorkplaceStatusRequest>
{
    public UpdateWorkplaceStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
