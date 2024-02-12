namespace Temp.Services.Groups.Models.Queries;

public class GetModeratorFreeGroupsRequest
{
    public int organizationId { get; set; }
    public int moderatorId { get; set; }
}
