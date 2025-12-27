namespace Temp.Services.Employees.Models.Queries;

public class GetEmployeesResponse
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsAppUserActive { get; set; }
}