namespace Temp.Domain.Models.Applications;

public class Application : BaseEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public Employee? User { get; set; }

    public int? ModeratorId { get; set; }
    public Employee? Moderator { get; set; }

    public int TeamId { get; set; }
    public Team? Team { get; set; }

    public string? Category { get; set; }

    public string? Content { get; set; }

    public bool Status { get; set; } = true;
    public DateTime StatusUpdatedAt { get; set; }
}