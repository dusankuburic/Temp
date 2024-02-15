using FluentValidation;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications.Models.Validators;

public class GetApplicationRequestValidator : AbstractValidator<GetApplicationRequest>
{
    public GetApplicationRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
