namespace Temp.Services.Applications.Models.Queries;

public class GetUserApplicationsResponse
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool Status { get; set; }
}