using System.Collections.Generic;

namespace Temp.Domain.Models;

public class EmploymentStatus
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Engagement> Engagements { get; set; }
}
