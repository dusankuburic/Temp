using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Domain.Models
{
    public class Engagement
    {
        public int Id {get; set;}

        public int EmployeeId {get; set;}
        public Employee Employee {get; set;}

        public int WorkplaceId {get; set;}
        public Workplace Workplace {get; set;}

        public int EmploymentStatusId {get; set;}
        public EmploymentStatus EmploymentStatus {get; set;}



       
        public DateTime DateFrom {get; set;}
        public DateTime DateTo {get; set;}
   
    }
}
