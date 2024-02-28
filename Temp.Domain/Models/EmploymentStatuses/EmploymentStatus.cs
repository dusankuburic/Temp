
namespace Temp.Domain.Models;

public class EmploymentStatus : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Engagement> Engagements { get; set; }
}
