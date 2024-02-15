using FluentValidation;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications.Models.Validators;

public class GetUserApplicationsRequestValidator : AbstractValidator<GetUserApplicationsRequest>
{
    public GetUserApplicationsRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{Id} is required")
            .NotNull();
    }
}
