﻿using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<RegisterResponse> Register(RegisterRequest request);
    Task<AssignRoleResponse> AssignRole(AssignRoleRequest request);
}