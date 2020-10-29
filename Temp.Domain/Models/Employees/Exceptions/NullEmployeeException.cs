using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Domain.Models.Employees.Exceptions
{
    public class NullEmployeeException : Exception
    {
        public NullEmployeeException() : base("The employee is null."){}
    }
}
