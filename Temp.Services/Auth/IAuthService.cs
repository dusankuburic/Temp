using Temp.Domain.Models.Identity;
using Temp.Services.Auth.Models.Commands;

namespace Temp.Services.Auth;

public interface IAuthService
{
    Task<LoginAppUserResponse> Login(LoginAppUserRequest request);
    Task<bool> Logout();
    Task<bool> CheckUsernameExists(string username);
    Task<AppUser> Register(RegisterAppUserRequest request);
    Task<AppUser> RemoveEmployeeRole(RemoveEmployeeRoleRequest request);
    Task<AppUser> UpdateEmployeeAccountStatus(int employeeId);
}