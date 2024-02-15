using FluentValidation;
using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Employees.Models.Validators;

public class RemoveEmployeeRoleRequestValidator : AbstractValidator<RemoveEmployeeRoleRequest>
{
    public RemoveEmployeeRoleRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
