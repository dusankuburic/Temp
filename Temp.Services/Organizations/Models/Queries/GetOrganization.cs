namespace Temp.Services.Organizations.Models.Queries;

public class GetOrganization
{
    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}