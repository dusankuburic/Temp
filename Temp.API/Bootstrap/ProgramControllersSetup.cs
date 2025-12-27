using FluentValidation.AspNetCore;
using Temp.Services.Applications.Models.Validators;

namespace Temp.API.Bootstrap;

public static class ProgramControllersSetup
{
    public static IMvcBuilder ConfigureSerialization(this IMvcBuilder builder) {
        builder.AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        return builder;
    }

    [Obsolete]
    public static IMvcBuilder ConfigureFluentValidation(this IMvcBuilder builder) {
        builder.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<CreateApplicationRequestValidator>());
        return builder;
    }
}