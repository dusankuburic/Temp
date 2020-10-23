
namespace Temp.Domain.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public Employee Employee {get; set;}
    }
}
