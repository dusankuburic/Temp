using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeWithoutEngagementStorageException : NotFoundException
{
    public EmployeeWithoutEngagementStorageException()
        : base("No employees found in storage without engagements") {
    }
}