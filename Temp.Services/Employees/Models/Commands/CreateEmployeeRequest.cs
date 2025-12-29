namespace Temp.Services.Employees.Models.Commands;

public class CreateEmployeeRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public int TeamId { get; set; }
}