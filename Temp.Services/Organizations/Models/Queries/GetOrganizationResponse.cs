namespace Temp.Services.Organizations.Models.Queries;

public class GetOrganizationResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool HasActiveGroup { get; set; }
}