using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Domain.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
