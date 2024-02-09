namespace Temp.Services.Groups.Models.Command;

public class CreateGroupRequest
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
}
