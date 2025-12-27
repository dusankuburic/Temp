namespace Temp.Services.Employees.Models.Queries;

public class GetEmployeesWithoutEngagementRequest
{
    private const int MaxPageSize = 20;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }

    public string? Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}