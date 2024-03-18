namespace Temp.Services.Auth.Models.Commands;

public class RegisterAppUserRequest
{
    public int EmployeeId { get; set; }

    public string DisplayName { get; set; }

    public string Role { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}
