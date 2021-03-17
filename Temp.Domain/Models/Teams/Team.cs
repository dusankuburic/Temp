using System.Collections.Generic;

namespace Temp.Domain.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int GroupId { get; set; } 
        public Group Group { get; set; }

        public ICollection<Employee> Employees { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
}
