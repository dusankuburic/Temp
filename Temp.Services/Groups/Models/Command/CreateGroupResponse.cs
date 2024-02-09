namespace Temp.Services.Groups.Models.Command;

public class CreateGroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
}
