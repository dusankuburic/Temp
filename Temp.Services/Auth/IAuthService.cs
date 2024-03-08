using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Auth;

public interface IAuthService
{
    Task<LoginAResponse> LoginAdmin(LoginAdminRequest request);
    Task<RegisterAdminResponse> RegisterAdmin(RegisterAdminRequest request);
    Task<Response> LoginUser(LoginUserRequest request);
    Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request);
    Task<LoginModeratorResponse> LoginModerator(LoginModeratorRequest request);
    Task<RegisterModeratorResponse> RegisterModerator(RegisterModeratorRequest request);
    Task<AssignRoleResponse> AssignRole(AssignRoleRequest request);
}
