namespace Temp.Services.Teams.Models.Commands;

public class UpdateTeamRequest
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string? Name { get; set; }
}