using Microsoft.EntityFrameworkCore;
using Temp.Domain.Models;

namespace Temp.Database.CompiledQueries;


public static class EmployeeQueries
{

    public static readonly Func<ApplicationDbContext, int, Task<Employee?>> GetByIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int id) =>
            ctx.Employees.FirstOrDefault(e => e.Id == id));


    public static readonly Func<ApplicationDbContext, string, Task<Employee?>> GetByAppUserIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string appUserId) =>
            ctx.Employees.FirstOrDefault(e => e.AppUserId == appUserId));


    public static readonly Func<ApplicationDbContext, string, Task<int>> GetIdByAppUserIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string appUserId) =>
            ctx.Employees
                .Where(e => e.AppUserId == appUserId)
                .Select(e => e.Id)
                .FirstOrDefault());


    public static readonly Func<ApplicationDbContext, string, Task<bool>> ExistsByAppUserIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string appUserId) =>
            ctx.Employees.Any(e => e.AppUserId == appUserId));


    public static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<Employee>> GetByTeamIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int teamId) =>
            ctx.Employees
                .AsNoTracking()
                .Where(e => e.TeamId == teamId));


    public static readonly Func<ApplicationDbContext, string, Task<int>> CountByRoleAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string role) =>
            ctx.Employees.Count(e => e.Role == role));
}