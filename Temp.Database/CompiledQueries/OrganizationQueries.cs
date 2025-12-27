using Microsoft.EntityFrameworkCore;
using Temp.Domain.Models;

namespace Temp.Database.CompiledQueries;

public static class OrganizationQueries
{
    public static readonly Func<ApplicationDbContext, int, Task<Organization?>> GetByIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int id) =>
            ctx.Organizations.FirstOrDefault(o => o.Id == id));

    public static readonly Func<ApplicationDbContext, IAsyncEnumerable<Organization>> GetActiveAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx) =>
            ctx.Organizations
                .AsNoTracking()
                .Where(o => o.IsActive));

    public static readonly Func<ApplicationDbContext, string, Task<bool>> ExistsByNameAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string name) =>
            ctx.Organizations.Any(o => o.Name == name));
}

public static class GroupQueries
{
    public static readonly Func<ApplicationDbContext, int, Task<Group?>> GetByIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int id) =>
            ctx.Groups.FirstOrDefault(g => g.Id == id));

    public static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<Group>> GetActiveByOrganizationIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int organizationId) =>
            ctx.Groups
                .AsNoTracking()
                .Where(g => g.OrganizationId == organizationId && g.IsActive));

    public static readonly Func<ApplicationDbContext, string, int, Task<bool>> ExistsByNameAndOrgAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string name, int organizationId) =>
            ctx.Groups.Any(g => g.Name == name && g.OrganizationId == organizationId));
}

public static class TeamQueries
{
    public static readonly Func<ApplicationDbContext, int, Task<Team?>> GetByIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int id) =>
            ctx.Teams.FirstOrDefault(t => t.Id == id));

    public static readonly Func<ApplicationDbContext, int, IAsyncEnumerable<Team>> GetActiveByGroupIdAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int groupId) =>
            ctx.Teams
                .AsNoTracking()
                .Where(t => t.GroupId == groupId && t.IsActive));

    public static readonly Func<ApplicationDbContext, string, int, Task<bool>> ExistsByNameAndGroupAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, string name, int groupId) =>
            ctx.Teams.Any(t => t.Name == name && t.GroupId == groupId));

    public static readonly Func<ApplicationDbContext, int, Task<bool>> AnyActiveInGroupAsync =
        EF.CompileAsyncQuery((ApplicationDbContext ctx, int groupId) =>
            ctx.Teams.Any(t => t.GroupId == groupId && t.IsActive));
}