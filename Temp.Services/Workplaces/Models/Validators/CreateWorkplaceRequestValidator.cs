using FluentValidation;
using Temp.Services.Workplaces.Models.Commands;

namespace Temp.Services.Workplaces.Models.Validators;

public class CreateWorkplaceRequestValidator : AbstractValidator<CreateWorkplaceRequest>
{
    public CreateWorkplaceRequestValidator() {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{Name} is required")
            .NotNull()
            .MaximumLength(90)
            .WithMessage("{Name} must not exceed 90 characters");
    }
}
