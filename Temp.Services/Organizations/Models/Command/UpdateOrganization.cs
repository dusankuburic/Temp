namespace Temp.Services.Organizations.Models.Command;

public class UpdateOrganization
{
    public class Request
    {
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}

