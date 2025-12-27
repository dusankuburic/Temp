using FluentValidation;
using Temp.Services.Organizations.Models.Commands;

namespace Temp.Services.Organizations.Models.Validators;

public class UpdateOrganizationStatusRequestValidator : AbstractValidator<UpdateOrganizationStatusRequest>
{
    public UpdateOrganizationStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}