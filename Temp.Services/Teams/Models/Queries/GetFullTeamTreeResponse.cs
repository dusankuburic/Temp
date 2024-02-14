namespace Temp.Services.Teams.Models.Queries;

public class GetFullTeamTreeResponse
{
    public int Id { get; set; }
    public string OrganizationName { get; set; }
    public int OrganizationId { get; set; }
    public string GroupName { get; set; }
    public string TeamName { get; set; }
}
