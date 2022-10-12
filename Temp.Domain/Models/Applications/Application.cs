namespace Temp.Domain.Models;

public class Application
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int TeamId { get; set; }
    public Team Team { get; set; }

    public int? ModeratorId { get; set; }
    public Moderator Moderator { get; set; }

    public string Category { get; set; }

    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool Status { get; set; }
    public DateTime StatusUpdatedAt { get; set; }
}
