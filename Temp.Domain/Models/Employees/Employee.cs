
namespace Temp.Domain.Models;

public class Employee : BaseEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Role { get; set; }

    public User User { get; set; }

    public Admin Admin { get; set; }

    public Moderator Moderator { get; set; }

    public ICollection<Engagement> Engagements { get; set; }

    public int? TeamId { get; set; }
    public Team Team { get; set; }
}
