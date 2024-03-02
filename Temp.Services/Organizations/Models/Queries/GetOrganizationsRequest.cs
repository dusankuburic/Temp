namespace Temp.Services.Organizations.Models.Queries;

public class GetOrganizationsRequest
{
    private const int MaxPageSize = 20;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }

    public string Name { get; set; }
}
