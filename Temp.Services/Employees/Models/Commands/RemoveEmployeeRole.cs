namespace Temp.Services.Employees.Models.Commands;

public class RemoveEmployeeRole
{
    public class Request
    {
        [Required]
        public int Id { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
