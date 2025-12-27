
namespace Temp.Domain.Models;

public class Group : BaseEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsActive { get; set; } = true;

    public int OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    public ICollection<Team>? Teams { get; set; }

    public bool HasActiveTeam { get; set; }

    public ICollection<ModeratorGroup>? ModeratorGroups { get; set; }
}