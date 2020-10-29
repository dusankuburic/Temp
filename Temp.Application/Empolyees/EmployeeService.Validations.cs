using System;
using System.Collections.Generic;
using System.Text;
using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models;

namespace Temp.Application.Empolyees
{
    public partial class CreateEmployee
    {

        private void ValidateEmployeeOnCreate(Employee employee)
        {
            ValidateEmployee(employee);
        }


        private void ValidateEmployee(Employee employee)
        {
            if(employee is null)
            {
               throw new NullEmployeeException();
            }
        }

    }
}
