namespace Temp.Services.Employees.Models.Command;

public class CreateEmployee
{
    public class Request
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public int TeamId { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
    }
}

