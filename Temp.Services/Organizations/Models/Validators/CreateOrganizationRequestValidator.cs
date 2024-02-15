using FluentValidation;
using Temp.Services.Organizations.Models.Commands;

namespace Temp.Services.Organizations.Models.Validators;

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator() {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{Name} is required")
            .NotNull()
            .MaximumLength(90)
            .WithMessage("{Name} must not exceed 90 characters");
    }
}
