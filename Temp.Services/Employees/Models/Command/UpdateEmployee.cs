namespace Temp.Services.Employees.Models.Command;

public class UpdateEmployee
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
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Status { get; set; }
    }
}
