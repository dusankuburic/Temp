using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Temp.Application.Empolyees
{
    public partial class EmployeeService
    {

        public void ValidateEmployeeOnCreate(Employee employee)
        {
            ValidateEmployee(employee);
            ValidateEmployeeStrings(employee);
        }


        public void ValidateEmployeeOnUpdate(Employee employee)
        {
            ValidateEmployee(employee);
            ValidateEmployeeStrings(employee);
        }



        public void ValidateEmployee(Employee employee)
        {
            if(employee is null)
            {
               throw new NullEmployeeException();
            }
        }

        public void ValidateGetEmployeViewModel(GetEmployee.EmployeeViewModel employee)
        {
            if(employee is null)
            {
               throw new NullEmployeeException();
            }
        }

        public void ValidateEmployeeStrings(Employee employee)
        {
            switch (employee)
            {
                case { } when IsInvalid(employee.FirstName):
                    throw new InvalidEmployeeException(
                        parameterName: nameof(employee.FirstName),
                        parameterValue: employee.FirstName);
                case {} when IsInvalid(employee.LastName):
                    throw new InvalidEmployeeException(
                        parameterName: nameof(employee.LastName),
                        parameterValue: employee.LastName);
            }
        }

        public void ValidateStorageEmployees(IEnumerable<GetEmployees.EmployeeViewModel> storageEmployees)
        {
            if(storageEmployees.Count() == 0)
            {
                throw new EmployeeEmptyStorageException();
            }
        }

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);

    }
}
