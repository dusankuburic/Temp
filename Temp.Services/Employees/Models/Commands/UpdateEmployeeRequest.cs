namespace Temp.Services.Employees.Models.Commands;

public class UpdateEmployeeRequest
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public int TeamId { get; set; }
}