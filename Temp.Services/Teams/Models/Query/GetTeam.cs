namespace Temp.Services.Teams.Models.Query;

public class GetTeam
{

    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
    }
}

