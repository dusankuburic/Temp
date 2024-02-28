
namespace Temp.Domain.Models;

public class Organization : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Group> Groups { get; set; }
}
