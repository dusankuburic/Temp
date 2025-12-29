namespace Temp.Services.Groups.Models.Commands;

public class CreateGroupRequest
{
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int OrganizationId { get; set; }
}