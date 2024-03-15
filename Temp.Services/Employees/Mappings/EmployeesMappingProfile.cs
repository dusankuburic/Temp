using Temp.Domain.Models;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees.Mappings;

public class EmployeesMappingProfile : Profile
{
    public EmployeesMappingProfile() {
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<Employee, CreateEmployeeResponse>();
        CreateMap<Employee, GetEmployeeResponse>();
        CreateMap<Employee, GetEmployeesResponse>();
        CreateMap<Employee, GetEmployeesWithoutEngagementResponse>();
        CreateMap<Employee, GetEmployeesWithEngagementResponse>();
        CreateMap<UpdateEmployeeRequest, Employee>()
            .AfterMap((a, b) => {
                b.UpdatedAt = DateTime.UtcNow;
            });

    }
}
