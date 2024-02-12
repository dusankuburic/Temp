namespace Temp.Services.Teams.Models.Commands;

public class UpdateTeam
{
    public class Request
    {
        [Required]
        public int GroupId { get; set; }

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
