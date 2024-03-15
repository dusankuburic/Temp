
using Temp.Domain.Models.Applications;

namespace Temp.Domain.Models;

public class Employee : BaseEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Role { get; set; }

    public string AppUserId { get; set; }
    public bool IsAppUserActive { get; set; }

    public ICollection<Engagement> Engagements { get; set; }
    public ICollection<ModeratorGroup> ModeratorGroups { get; set; }

    public ICollection<Application> Applications { get; set; }
    public ICollection<Application> ModeratorApplications { get; set; }

    public int? TeamId { get; set; }
    public Team Team { get; set; }
}
