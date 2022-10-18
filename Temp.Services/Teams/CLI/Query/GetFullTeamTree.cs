namespace Temp.Services.Teams.CLI.Query;

public class GetFullTeamTree
{
    public class TeamTreeViewModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationId { get; set; }
        public string GroupName { get; set; }
        public string TeamName { get; set; }
    }
}

