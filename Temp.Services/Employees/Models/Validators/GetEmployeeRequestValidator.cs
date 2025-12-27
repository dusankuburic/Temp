using FluentValidation;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees.Models.Validators;

public class GetEmployeeRequestValidator : AbstractValidator<GetEmployeeRequest>
{
    public GetEmployeeRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}