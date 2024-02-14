using Temp.Domain.Models.Applications;
using Temp.Services.Applications.Models.Commands;

namespace Temp.Services.Applications.Mapping;

public class CreateApplicationProfile : Profile
{
    public CreateApplicationProfile() {
        CreateMap<CreateApplicationRequest, Application>()
            .AfterMap((req, application) => {
                application.CreatedAt = DateTime.Now;
            });
    }
}

