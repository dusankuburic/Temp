namespace Temp.Services.Groups.Models.Queries;

public class GetModeratorFreeGroupsRequest
{
    public int OrganizationId { get; set; }
    public int ModeratorId { get; set; }
}