using Temp.Domain.Models.Applications;

namespace Temp.Domain.Models;

public class Team : BaseEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsActive { get; set; } = true;

    public int GroupId { get; set; }
    public Group? Group { get; set; }

    public ICollection<Employee>? Employees { get; set; }

    public ICollection<Application>? Applications { get; set; }
}