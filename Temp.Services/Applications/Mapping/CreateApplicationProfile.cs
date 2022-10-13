using AutoMapper;
using Temp.Domain.Models.Applications;
using Temp.Services.Applications.CLI.Command;

namespace Temp.Services.Applications.Mapping;

public class CreateApplicationProfile : Profile
{
    public CreateApplicationProfile() {
        CreateMap<CreateApplication.Request, Application>()
            .AfterMap((req, application) => {
                application.CreatedAt = DateTime.Now;
            });
    }
}

