using System;

namespace Temp.Domain.Models.Employees.Exceptions;

public class EmployeeWithoutEngagementStorageException : Exception
{
    public EmployeeWithoutEngagementStorageException() : base("No employees found in storage without engagements") {

    }
}
