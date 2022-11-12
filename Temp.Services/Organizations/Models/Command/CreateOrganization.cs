namespace Temp.Services.Organizations.Models.Command;

public class CreateOrganization
{
    public class Request
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}

