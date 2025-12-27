using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeWithEngagementStorageException : NotFoundException
{
    public EmployeeWithEngagementStorageException()
        : base("No employees found in storage with engagements") {
    }
}