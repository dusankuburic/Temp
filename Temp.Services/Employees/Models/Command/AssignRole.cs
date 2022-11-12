namespace Temp.Services.Employees.Models.Command;
public class AssignRole
{
    public class Request
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }
        [Required]
        [MaxLength(30)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }

    public class Response
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
