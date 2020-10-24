using System.Collections.Generic;

namespace Temp.Domain.Models
{
    public class Employee
    {
        public int Id {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public string Role {get; set;}
        public ICollection<User> Users {get; set; }
        public ICollection<Admin> Admins {get; set;}
    }
}
