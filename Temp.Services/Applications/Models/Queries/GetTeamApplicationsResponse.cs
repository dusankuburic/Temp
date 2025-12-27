namespace Temp.Services.Applications.Models.Queries;

public class GetTeamApplicationsResponse
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool Status { get; set; }
}