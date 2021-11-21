namespace Temp.Domain.Models
{
    public class ModeratorGroup
    {
        public int ModeratorId { get; set; }
        public Moderator Moderator { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}