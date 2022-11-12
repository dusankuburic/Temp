namespace Temp.Services.Employees.Exceptions;

public class EmployeeWithEngagementStorageException : Exception
{
    public EmployeeWithEngagementStorageException()
        : base("No employees found in storage with engagements") { }
}
