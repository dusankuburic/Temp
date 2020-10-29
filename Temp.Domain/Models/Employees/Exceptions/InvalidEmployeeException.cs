using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Domain.Models.Employees.Exceptions
{
    public class InvalidEmployeeException : Exception
    {
        public InvalidEmployeeException(string parameterName, object parameterValue)
            : base($"Invalid Employee, " +
                  $"ParameterName : {parameterName}, " +
                  $"ParameterValue: {parameterValue}.")
            {}
        
    }
}
