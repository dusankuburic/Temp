using FluentValidation;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations.Models.Validators;

public class GetOrganizationRequestValidator : AbstractValidator<GetOrganizationRequest>
{
    public GetOrganizationRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
