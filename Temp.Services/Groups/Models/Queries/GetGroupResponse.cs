namespace Temp.Services.Groups.Models.Queries;

public class GetGroupResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int OrganizationId { get; set; }
}