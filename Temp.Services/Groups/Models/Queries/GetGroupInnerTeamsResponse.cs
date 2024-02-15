namespace Temp.Services.Groups.Models.Queries;

public class GetGroupInnerTeamsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<InnerTeam> Teams { get; set; }
}

public class InnerTeam
{
    public int Id { get; set; }
    public string Name { get; set; }
}
