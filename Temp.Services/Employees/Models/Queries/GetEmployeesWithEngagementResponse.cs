namespace Temp.Services.Employees.Models.Queries;

public class GetEmployeesWithEngagementResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }

    public IEnumerable<string> Workplace { get; set; }
    public IEnumerable<string> EmploymentStatus { get; set; }
    public IEnumerable<int> Salary { get; set; }
}
