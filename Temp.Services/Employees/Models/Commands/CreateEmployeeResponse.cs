namespace Temp.Services.Employees.Models.Commands;

public class CreateEmployeeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? TeamId { get; set; }
}
