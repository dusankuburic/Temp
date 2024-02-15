using FluentValidation;
using Temp.Services.Applications.Models.Commands;

namespace Temp.Services.Applications.Models.Validators;

public class CreateApplicationRequestValidator : AbstractValidator<CreateApplicationRequest>
{
    public CreateApplicationRequestValidator() {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("{UserId} is required")
            .NotNull();

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("{TeamId} is required")
            .NotNull();

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("{Content} is required")
            .NotNull()
            .MaximumLength(300)
            .WithMessage("{Content} must not exceed 300 characters");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("{Category} is required")
            .NotNull();
    }
}
