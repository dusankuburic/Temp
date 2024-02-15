using FluentValidation;
using Temp.Services.Groups.Models.Commands;

namespace Temp.Services.Groups.Models.Validators;

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator() {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("{OrganizationId} is required")
            .NotNull();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{Name} is required")
            .NotNull()
            .MaximumLength(90)
            .WithMessage("{Name} must not exceed 90 characters");
    }
}
