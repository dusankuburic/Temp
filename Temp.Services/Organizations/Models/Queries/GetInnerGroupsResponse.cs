using Temp.Services._Helpers;

namespace Temp.Services.Organizations.Models.Queries;

public class GetInnerGroupsResponse
{
    public string Name { get; set; }
    public IEnumerable<InnerGroup> Groups { get; set; }
}

public class GetPagedInnerGroupsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public PagedList<InnerGroup> Groups { get; set; }
}

public class InnerGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
}
