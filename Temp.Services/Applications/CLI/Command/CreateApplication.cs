namespace Temp.Services.Applications.CLI.Command
{
    public class CreateApplication
    {
        public class Request
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public int TeamId { get; set; }
            [Required]
            [MaxLength(600)]
            public string Content { get; set; }
            [Required]
            public string Category { get; set; }

        }

        public class Response
        {
            public bool Status { get; set; }
        }
    }
}
