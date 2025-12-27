
namespace Temp.Domain.Models;

public class ModeratorGroup : BaseEntity
{
    public int ModeratorId { get; set; }
    public Employee? Moderator { get; set; }

    public int GroupId { get; set; }
    public Group? Group { get; set; }
}