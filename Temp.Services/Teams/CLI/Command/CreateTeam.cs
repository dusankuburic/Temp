namespace Temp.Services.Teams.CLI.Command
{
    public class CreateTeam
    {
        public class Request
        {
            [Required]
            public int Id { get; set; }

            [Required]
            public int GroupId { get; set; }

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
}
