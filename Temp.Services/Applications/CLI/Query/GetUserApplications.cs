namespace Temp.Services.Applications.CLI.Query;

public class GetUserApplications
{
    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}

