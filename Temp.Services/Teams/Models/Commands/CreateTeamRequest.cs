namespace Temp.Services.Teams.Models.Commands;

public class CreateTeamRequest
{
    public int GroupId { get; set; }
    public string? Name { get; set; }
}