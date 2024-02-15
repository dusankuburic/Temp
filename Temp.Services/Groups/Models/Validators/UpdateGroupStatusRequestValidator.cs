using FluentValidation;
using Temp.Services.Groups.Models.Commands;

namespace Temp.Services.Groups.Models.Validators;

public class UpdateGroupStatusRequestValidator : AbstractValidator<UpdateGroupStatusRequest>
{
    public UpdateGroupStatusRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
