namespace Temp.Services.Groups.Models.Query;

public class GetModeratorFreeGroupsRequest
{
    public int organizationId { get; set; }
    public int moderatorId { get; set; }
}
