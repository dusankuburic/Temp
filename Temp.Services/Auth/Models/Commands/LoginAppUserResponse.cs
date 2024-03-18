namespace Temp.Services.Auth.Models.Commands;

public class LoginAppUserResponse
{
    public string Token { get; set; }
    public LoginAppUser User { get; set; }
}

public class LoginAppUser
{
    public int Id { get; set; }
    public string Username { get; set; }
}
