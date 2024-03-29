﻿namespace Temp.Services.Employees.Exceptions;

public class EmployeeDependencyException : Exception
{
    public EmployeeDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) {

    }
}
