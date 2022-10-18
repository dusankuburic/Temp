namespace Temp.Services.Applications.CLI.Command;

public class UpdateApplicationStatus
{
    public class Request
    {
        public int ModeratorId { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public bool Status { get; set; }
    }
}