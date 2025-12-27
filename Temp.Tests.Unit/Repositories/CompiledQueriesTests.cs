using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Database.CompiledQueries;
using Temp.Domain.Models;

namespace Temp.Tests.Unit.Repositories;

public sealed class CompiledQueriesTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public CompiledQueriesTests() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        SeedData();
    }

    public void Dispose() {
        _context.Dispose();
    }

    private void SeedData() {

        var org1 = new Organization { Id = 1, Name = "Org Alpha", IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var org2 = new Organization { Id = 2, Name = "Org Beta", IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var org3 = new Organization { Id = 3, Name = "Org Inactive", IsActive = false, CreatedBy = "test", UpdatedBy = "test" };


        var group1 = new Group { Id = 1, Name = "Group 1", OrganizationId = 1, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var group2 = new Group { Id = 2, Name = "Group 2", OrganizationId = 1, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var group3 = new Group { Id = 3, Name = "Inactive Group", OrganizationId = 1, IsActive = false, CreatedBy = "test", UpdatedBy = "test" };
        var group4 = new Group { Id = 4, Name = "Group 1", OrganizationId = 2, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };


        var team1 = new Team { Id = 1, Name = "Team A", GroupId = 1, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var team2 = new Team { Id = 2, Name = "Team B", GroupId = 1, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };
        var team3 = new Team { Id = 3, Name = "Inactive Team", GroupId = 1, IsActive = false, CreatedBy = "test", UpdatedBy = "test" };
        var team4 = new Team { Id = 4, Name = "Team A", GroupId = 2, IsActive = true, CreatedBy = "test", UpdatedBy = "test" };


        var emp1 = new Employee { Id = 1, FirstName = "John", LastName = "Doe", AppUserId = "user-001", TeamId = 1, Role = "Admin", CreatedBy = "test", UpdatedBy = "test" };
        var emp2 = new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", AppUserId = "user-002", TeamId = 1, Role = "User", CreatedBy = "test", UpdatedBy = "test" };
        var emp3 = new Employee { Id = 3, FirstName = "Bob", LastName = "Wilson", AppUserId = "user-003", TeamId = 2, Role = "User", CreatedBy = "test", UpdatedBy = "test" };
        var emp4 = new Employee { Id = 4, FirstName = "Alice", LastName = "Brown", AppUserId = "user-004", TeamId = 1, Role = "Admin", CreatedBy = "test", UpdatedBy = "test" };

        _context.Organizations.AddRange(org1, org2, org3);
        _context.Groups.AddRange(group1, group2, group3, group4);
        _context.Teams.AddRange(team1, team2, team3, team4);
        _context.Employees.AddRange(emp1, emp2, emp3, emp4);
        _context.SaveChanges();
    }



    [Fact]
    public async Task EmployeeQueries_GetByIdAsync_ReturnsEmployee() {
        var result = await EmployeeQueries.GetByIdAsync(_context, 1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task EmployeeQueries_GetByIdAsync_WithInvalidId_ReturnsNull() {
        var result = await EmployeeQueries.GetByIdAsync(_context, 999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task EmployeeQueries_GetByAppUserIdAsync_ReturnsEmployee() {
        var result = await EmployeeQueries.GetByAppUserIdAsync(_context, "user-001");

        result.Should().NotBeNull();
        result!.AppUserId.Should().Be("user-001");
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task EmployeeQueries_GetByAppUserIdAsync_WithInvalidId_ReturnsNull() {
        var result = await EmployeeQueries.GetByAppUserIdAsync(_context, "invalid-user");

        result.Should().BeNull();
    }

    [Fact]
    public async Task EmployeeQueries_GetIdByAppUserIdAsync_ReturnsEmployeeId() {
        var result = await EmployeeQueries.GetIdByAppUserIdAsync(_context, "user-002");

        result.Should().Be(2);
    }

    [Fact]
    public async Task EmployeeQueries_GetIdByAppUserIdAsync_WithInvalidId_ReturnsDefault() {
        var result = await EmployeeQueries.GetIdByAppUserIdAsync(_context, "invalid-user");

        result.Should().Be(0);
    }

    [Fact]
    public async Task EmployeeQueries_ExistsByAppUserIdAsync_WhenExists_ReturnsTrue() {
        var result = await EmployeeQueries.ExistsByAppUserIdAsync(_context, "user-001");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmployeeQueries_ExistsByAppUserIdAsync_WhenNotExists_ReturnsFalse() {
        var result = await EmployeeQueries.ExistsByAppUserIdAsync(_context, "invalid-user");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmployeeQueries_GetByTeamIdAsync_ReturnsEmployeesInTeam() {
        var results = new List<Employee>();
        await foreach (var employee in EmployeeQueries.GetByTeamIdAsync(_context, 1)) {
            results.Add(employee);
        }

        results.Should().HaveCount(3);
        results.Should().OnlyContain(e => e.TeamId == 1);
    }

    [Fact]
    public async Task EmployeeQueries_GetByTeamIdAsync_WithNoEmployees_ReturnsEmpty() {
        var results = new List<Employee>();
        await foreach (var employee in EmployeeQueries.GetByTeamIdAsync(_context, 999)) {
            results.Add(employee);
        }

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task EmployeeQueries_CountByRoleAsync_ReturnsCorrectCount() {
        var adminCount = await EmployeeQueries.CountByRoleAsync(_context, "Admin");
        var userCount = await EmployeeQueries.CountByRoleAsync(_context, "User");

        adminCount.Should().Be(2);
        userCount.Should().Be(2);
    }

    [Fact]
    public async Task EmployeeQueries_CountByRoleAsync_WithNonExistentRole_ReturnsZero() {
        var result = await EmployeeQueries.CountByRoleAsync(_context, "NonExistent");

        result.Should().Be(0);
    }

    [Fact]
    public async Task OrganizationQueries_GetByIdAsync_ReturnsOrganization() {
        var result = await OrganizationQueries.GetByIdAsync(_context, 1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Org Alpha");
    }

    [Fact]
    public async Task OrganizationQueries_GetByIdAsync_WithInvalidId_ReturnsNull() {
        var result = await OrganizationQueries.GetByIdAsync(_context, 999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task OrganizationQueries_GetActiveAsync_ReturnsOnlyActiveOrganizations() {
        var results = new List<Organization>();
        await foreach (var org in OrganizationQueries.GetActiveAsync(_context)) {
            results.Add(org);
        }

        results.Should().HaveCount(2);
        results.Should().OnlyContain(o => o.IsActive);
        results.Should().NotContain(o => o.Name == "Org Inactive");
    }

    [Fact]
    public async Task OrganizationQueries_ExistsByNameAsync_WhenExists_ReturnsTrue() {
        var result = await OrganizationQueries.ExistsByNameAsync(_context, "Org Alpha");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task OrganizationQueries_ExistsByNameAsync_WhenNotExists_ReturnsFalse() {
        var result = await OrganizationQueries.ExistsByNameAsync(_context, "NonExistent Org");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GroupQueries_GetByIdAsync_ReturnsGroup() {
        var result = await GroupQueries.GetByIdAsync(_context, 1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Group 1");
    }

    [Fact]
    public async Task GroupQueries_GetByIdAsync_WithInvalidId_ReturnsNull() {
        var result = await GroupQueries.GetByIdAsync(_context, 999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GroupQueries_GetActiveByOrganizationIdAsync_ReturnsActiveGroupsInOrg() {
        var results = new List<Group>();
        await foreach (var group in GroupQueries.GetActiveByOrganizationIdAsync(_context, 1)) {
            results.Add(group);
        }

        results.Should().HaveCount(2);
        results.Should().OnlyContain(g => g.IsActive && g.OrganizationId == 1);
        results.Should().NotContain(g => g.Name == "Inactive Group");
    }

    [Fact]
    public async Task GroupQueries_GetActiveByOrganizationIdAsync_WithNoActiveGroups_ReturnsEmpty() {
        var results = new List<Group>();
        await foreach (var group in GroupQueries.GetActiveByOrganizationIdAsync(_context, 3)) {
            results.Add(group);
        }

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GroupQueries_ExistsByNameAndOrgAsync_WhenExists_ReturnsTrue() {
        var result = await GroupQueries.ExistsByNameAndOrgAsync(_context, "Group 1", 1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GroupQueries_ExistsByNameAndOrgAsync_SameNameDifferentOrg_ReturnsFalse() {
        var result = await GroupQueries.ExistsByNameAndOrgAsync(_context, "Group 1", 3);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GroupQueries_ExistsByNameAndOrgAsync_WhenNotExists_ReturnsFalse() {
        var result = await GroupQueries.ExistsByNameAndOrgAsync(_context, "NonExistent Group", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task TeamQueries_GetByIdAsync_ReturnsTeam() {
        var result = await TeamQueries.GetByIdAsync(_context, 1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Team A");
    }

    [Fact]
    public async Task TeamQueries_GetByIdAsync_WithInvalidId_ReturnsNull() {
        var result = await TeamQueries.GetByIdAsync(_context, 999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TeamQueries_GetActiveByGroupIdAsync_ReturnsActiveTeamsInGroup() {
        var results = new List<Team>();
        await foreach (var team in TeamQueries.GetActiveByGroupIdAsync(_context, 1)) {
            results.Add(team);
        }

        results.Should().HaveCount(2);
        results.Should().OnlyContain(t => t.IsActive && t.GroupId == 1);
        results.Should().NotContain(t => t.Name == "Inactive Team");
    }

    [Fact]
    public async Task TeamQueries_GetActiveByGroupIdAsync_WithNoActiveTeams_ReturnsEmpty() {
        var results = new List<Team>();
        await foreach (var team in TeamQueries.GetActiveByGroupIdAsync(_context, 999)) {
            results.Add(team);
        }

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task TeamQueries_ExistsByNameAndGroupAsync_WhenExists_ReturnsTrue() {
        var result = await TeamQueries.ExistsByNameAndGroupAsync(_context, "Team A", 1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task TeamQueries_ExistsByNameAndGroupAsync_SameNameDifferentGroup_ReturnsFalse() {
        var result = await TeamQueries.ExistsByNameAndGroupAsync(_context, "Team A", 3);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task TeamQueries_ExistsByNameAndGroupAsync_WhenNotExists_ReturnsFalse() {
        var result = await TeamQueries.ExistsByNameAndGroupAsync(_context, "NonExistent Team", 1);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CompiledQueries_AreReusable() {
        var result1 = await EmployeeQueries.GetByIdAsync(_context, 1);
        var result2 = await EmployeeQueries.GetByIdAsync(_context, 2);
        var result3 = await EmployeeQueries.GetByIdAsync(_context, 3);
        var result4 = await EmployeeQueries.GetByIdAsync(_context, 1);

        result1!.Id.Should().Be(1);
        result2!.Id.Should().Be(2);
        result3!.Id.Should().Be(3);
        result4!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CompiledQueries_WorkWithDifferentParameters() {
        var exists1 = await EmployeeQueries.ExistsByAppUserIdAsync(_context, "user-001");
        var exists2 = await EmployeeQueries.ExistsByAppUserIdAsync(_context, "user-002");
        var exists3 = await EmployeeQueries.ExistsByAppUserIdAsync(_context, "invalid");

        exists1.Should().BeTrue();
        exists2.Should().BeTrue();
        exists3.Should().BeFalse();
    }

    [Fact]
    public async Task CompiledQueries_ReturnConsistentResults() {
        var count1 = await EmployeeQueries.CountByRoleAsync(_context, "Admin");
        var count2 = await EmployeeQueries.CountByRoleAsync(_context, "Admin");

        count1.Should().Be(count2);
    }


}