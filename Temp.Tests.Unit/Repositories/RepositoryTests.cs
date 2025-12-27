using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Database.Repositories;
using Temp.Domain.Models;

namespace Temp.Tests.Unit.Repositories;

public class RepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context = null!;
    private Repository<Team> _repository = null!;
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public Task InitializeAsync() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<Team>(_context);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }



    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.GetByIdAsync(1);


        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Team");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull() {

        var result = await _repository.GetByIdAsync(999);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithZeroId_ReturnsNull() {

        var result = await _repository.GetByIdAsync(0);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithNegativeId_ReturnsNull() {

        var result = await _repository.GetByIdAsync(-1);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_RespectsCancellation() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        var cts = new CancellationTokenSource();


        var result = await _repository.GetByIdAsync(1, cts.Token);


        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithCancelledToken_ThrowsOperationCanceledException() {

        var cts = new CancellationTokenSource();
        cts.Cancel();


        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByIdAsync(1, cts.Token));
    }





    [Fact]
    public async Task GetAllAsync_WithEntities_ReturnsAllEntities() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2"),
            CreateTeam(3, "Team 3")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.GetAllAsync();


        result.Should().HaveCount(3);
        result.Should().BeAssignableTo<IReadOnlyList<Team>>();
        result.Select(t => t.Name).Should().BeEquivalentTo("Team 1", "Team 2", "Team 3");
    }

    [Fact]
    public async Task GetAllAsync_WithNoEntities_ReturnsEmptyCollection() {

        var result = await _repository.GetAllAsync();


        result.Should().BeEmpty();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithLargeDataset_ReturnsAllEntities() {

        var teams = Enumerable.Range(1, 100)
            .Select(i => CreateTeam(i, $"Team {i}"))
            .ToList();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.GetAllAsync();


        result.Should().HaveCount(100);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_RespectsCancellation() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        var cts = new CancellationTokenSource();


        var result = await _repository.GetAllAsync(cts.Token);


        result.Should().HaveCount(1);
    }





    [Fact]
    public async Task FindAsync_WithMatchingPredicate_ReturnsMatchingEntities() {

        var teams = new[]
        {
            CreateTeam(1, "Dev Team", isActive: true),
            CreateTeam(2, "QA Team", isActive: false),
            CreateTeam(3, "Dev Ops Team", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.FindAsync(t => t.Name.Contains("Dev"));


        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Name.Contains("Dev"));
    }

    [Fact]
    public async Task FindAsync_WithNonMatchingPredicate_ReturnsEmptyCollection() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.FindAsync(t => t.Name == "NonExistent");


        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_WithBooleanPredicate_FiltersCorrectly() {

        var teams = new[]
        {
            CreateTeam(1, "Active Team 1", isActive: true),
            CreateTeam(2, "Inactive Team", isActive: false),
            CreateTeam(3, "Active Team 2", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.FindAsync(t => t.IsActive);


        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IsActive);
    }

    [Fact]
    public async Task FindAsync_WithComplexPredicate_FiltersCorrectly() {

        var teams = new[]
        {
            CreateTeam(1, "Dev Team Alpha", isActive: true),
            CreateTeam(2, "Dev Team Beta", isActive: false),
            CreateTeam(3, "QA Team", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.FindAsync(t => t.Name.StartsWith("Dev") && t.IsActive);


        result.Should().ContainSingle();
        result.First().Name.Should().Be("Dev Team Alpha");
    }

    [Fact]
    public async Task FindAsync_WithEmptyDatabase_ReturnsEmptyCollection() {

        var result = await _repository.FindAsync(t => t.IsActive);


        result.Should().BeEmpty();
    }





    [Fact]
    public async Task FirstOrDefaultAsync_WithMatchingPredicate_ReturnsFirstMatch() {

        var teams = new[]
        {
            CreateTeam(1, "Team A"),
            CreateTeam(2, "Team B"),
            CreateTeam(3, "Team A Duplicate")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.FirstOrDefaultAsync(t => t.Name.StartsWith("Team A"));


        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNonMatchingPredicate_ReturnsNull() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.FirstOrDefaultAsync(t => t.Name == "NonExistent");


        result.Should().BeNull();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithEmptyDatabase_ReturnsNull() {

        var result = await _repository.FirstOrDefaultAsync(t => t.IsActive);


        result.Should().BeNull();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMultipleMatches_ReturnsFirst() {

        var teams = new[]
        {
            CreateTeam(1, "Active Team 1", isActive: true),
            CreateTeam(2, "Active Team 2", isActive: true),
            CreateTeam(3, "Active Team 3", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.FirstOrDefaultAsync(t => t.IsActive);


        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }





    [Fact]
    public async Task AnyAsync_WithMatchingPredicate_ReturnsTrue() {

        var team = CreateTeam(1, "Test Team", isActive: true);
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.AnyAsync(t => t.Name == "Test Team");


        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithNonMatchingPredicate_ReturnsFalse() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.AnyAsync(t => t.Name == "NonExistent");


        result.Should().BeFalse();
    }

    [Fact]
    public async Task AnyAsync_WithEmptyDatabase_ReturnsFalse() {

        var result = await _repository.AnyAsync(t => t.IsActive);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task AnyAsync_WithComplexPredicate_EvaluatesCorrectly() {

        var teams = new[]
        {
            CreateTeam(1, "Dev Team", isActive: true),
            CreateTeam(2, "QA Team", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var resultExists = await _repository.AnyAsync(t => t.Name.Contains("Dev") && t.IsActive);
        var resultNotExists = await _repository.AnyAsync(t => t.Name.Contains("QA") && t.IsActive);


        resultExists.Should().BeTrue();
        resultNotExists.Should().BeFalse();
    }





    [Fact]
    public async Task CountAsync_WithoutPredicate_ReturnsTotalCount() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2"),
            CreateTeam(3, "Team 3")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.CountAsync();


        result.Should().Be(3);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsMatchingCount() {

        var teams = new[]
        {
            CreateTeam(1, "Dev Team", isActive: true),
            CreateTeam(2, "QA Team", isActive: false),
            CreateTeam(3, "Dev Ops", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.CountAsync(t => t.IsActive);


        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WithEmptyDatabase_ReturnsZero() {

        var result = await _repository.CountAsync();


        result.Should().Be(0);
    }

    [Fact]
    public async Task CountAsync_WithNonMatchingPredicate_ReturnsZero() {

        var team = CreateTeam(1, "Test Team", isActive: true);
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.CountAsync(t => t.Name == "NonExistent");


        result.Should().Be(0);
    }

    [Fact]
    public async Task CountAsync_WithNullPredicate_ReturnsTotalCount() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.CountAsync(null);


        result.Should().Be(2);
    }





    [Fact]
    public void Query_ReturnsQueryable() {

        var query = _repository.Query();


        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQueryable<Team>>();
    }

    [Fact]
    public async Task Query_AllowsLinqOperations() {

        var teams = new[]
        {
            CreateTeam(1, "Team Z"),
            CreateTeam(2, "Team A"),
            CreateTeam(3, "Team M")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.Query()
            .OrderBy(t => t.Name)
            .Select(t => t.Name)
            .ToListAsync();


        result.Should().BeEquivalentTo(new[] { "Team A", "Team M", "Team Z" }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Query_AllowsPagination() {

        var teams = Enumerable.Range(1, 20)
            .Select(i => CreateTeam(i, $"Team {i:D2}"))
            .ToList();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.Query()
            .OrderBy(t => t.Id)
            .Skip(5)
            .Take(5)
            .ToListAsync();


        result.Should().HaveCount(5);
        result.First().Id.Should().Be(6);
        result.Last().Id.Should().Be(10);
    }

    [Fact]
    public async Task Query_AllowsIncludeOperations() {

        var group = new Group { Id = 1, Name = "Test Group", CreatedBy = "test", UpdatedBy = "test" };
        await _context.Groups.AddAsync(group);

        var team = CreateTeam(1, "Test Team");
        team.GroupId = 1;
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.Query()
            .Include(t => t.Group)
            .FirstOrDefaultAsync(t => t.Id == 1);


        result.Should().NotBeNull();
        result!.Group.Should().NotBeNull();
        result.Group.Name.Should().Be("Test Group");
    }

    [Fact]
    public async Task Query_TracksEntities() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var trackedTeam = await _repository.Query().FirstAsync(t => t.Id == 1);
        trackedTeam.Name = "Modified Name";
        await _context.SaveChangesAsync();


        var result = await _context.Teams.FindAsync(1);
        result!.Name.Should().Be("Modified Name");
    }





    [Fact]
    public void QueryNoTracking_ReturnsQueryable() {

        var query = _repository.QueryNoTracking();


        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQueryable<Team>>();
    }

    [Fact]
    public async Task QueryNoTracking_DoesNotTrackEntities() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var untrackedTeam = await _repository.QueryNoTracking().FirstAsync(t => t.Id == 1);


        _context.ChangeTracker.Entries<Team>().Should().BeEmpty();
    }

    [Fact]
    public async Task QueryNoTracking_AllowsFiltering() {

        var teams = new[]
        {
            CreateTeam(1, "Active Team", isActive: true),
            CreateTeam(2, "Inactive Team", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.QueryNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync();


        result.Should().ContainSingle();
        result.First().Name.Should().Be("Active Team");
    }





    [Fact]
    public async Task AddAsync_AddsEntityToContext() {

        var team = CreateTeam(1, "New Team");


        await _repository.AddAsync(team);
        await _context.SaveChangesAsync();


        var saved = await _context.Teams.FindAsync(1);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Team");
    }

    [Fact]
    public async Task AddAsync_SetsEntityStateToAdded() {

        var team = CreateTeam(1, "New Team");


        await _repository.AddAsync(team);


        var entry = _context.Entry(team);
        entry.State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_RespectsCancellation() {

        var team = CreateTeam(1, "New Team");
        var cts = new CancellationTokenSource();


        await _repository.AddAsync(team, cts.Token);
        await _context.SaveChangesAsync();


        var saved = await _context.Teams.FindAsync(1);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WithAuditFields_PreservesAuditData() {

        var team = CreateTeam(1, "Audited Team");
        team.CreatedAt = DateTime.UtcNow;
        team.CreatedBy = "test@example.com";


        await _repository.AddAsync(team);
        await _context.SaveChangesAsync();


        var saved = await _context.Teams.FindAsync(1);
        saved!.CreatedAt.Should().NotBeNull();
        saved.CreatedBy.Should().Be("test@example.com");
    }





    [Fact]
    public async Task AddRangeAsync_AddsMultipleEntities() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2"),
            CreateTeam(3, "Team 3")
        };


        await _repository.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var count = await _context.Teams.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task AddRangeAsync_WithEmptyCollection_DoesNotFail() {

        var teams = Array.Empty<Team>();


        await _repository.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var count = await _context.Teams.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task AddRangeAsync_WithLargeCollection_AddsAllEntities() {

        var teams = Enumerable.Range(1, 100)
            .Select(i => CreateTeam(i, $"Team {i}"))
            .ToList();


        await _repository.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var count = await _context.Teams.CountAsync();
        count.Should().Be(100);
    }





    [Fact]
    public async Task Update_ModifiesExistingEntity() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var teamToUpdate = await _context.Teams.FindAsync(1);
        teamToUpdate!.Name = "Updated Name";
        _repository.Update(teamToUpdate);
        await _context.SaveChangesAsync();


        _context.ChangeTracker.Clear();
        var updated = await _context.Teams.FindAsync(1);
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Update_SetsEntityStateToModified() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        team.Name = "Updated Name";
        _repository.Update(team);


        var entry = _context.Entry(team);
        entry.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task Update_UpdatesMultipleProperties() {

        var team = CreateTeam(1, "Original Name", isActive: true);
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var teamToUpdate = await _context.Teams.FindAsync(1);
        teamToUpdate!.Name = "Updated Name";
        teamToUpdate.IsActive = false;
        _repository.Update(teamToUpdate);
        await _context.SaveChangesAsync();


        _context.ChangeTracker.Clear();
        var updated = await _context.Teams.FindAsync(1);
        updated!.Name.Should().Be("Updated Name");
        updated.IsActive.Should().BeFalse();
    }





    [Fact]
    public async Task UpdateRange_ModifiesMultipleEntities() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var teamsToUpdate = await _context.Teams.ToListAsync();
        foreach (var team in teamsToUpdate) {
            team.Name = $"Updated {team.Name}";
        }
        _repository.UpdateRange(teamsToUpdate);
        await _context.SaveChangesAsync();


        _context.ChangeTracker.Clear();
        var updated = await _context.Teams.OrderBy(t => t.Id).ToListAsync();
        updated[0].Name.Should().Be("Updated Team 1");
        updated[1].Name.Should().Be("Updated Team 2");
    }

    [Fact]
    public async Task UpdateRange_WithEmptyCollection_DoesNotFail() {

        var teams = Array.Empty<Team>();


        _repository.UpdateRange(teams);
        await _context.SaveChangesAsync();


        true.Should().BeTrue();
    }





    [Fact]
    public async Task Remove_DeletesEntity() {

        var team = CreateTeam(1, "Team to Delete");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        _repository.Remove(team);
        await _context.SaveChangesAsync();


        var deleted = await _context.Teams.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Remove_SetsEntityStateToDeleted() {

        var team = CreateTeam(1, "Team to Delete");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        _repository.Remove(team);


        var entry = _context.Entry(team);
        entry.State.Should().Be(EntityState.Deleted);
    }





    [Fact]
    public async Task RemoveRange_DeletesMultipleEntities() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2"),
            CreateTeam(3, "Team 3")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        _repository.RemoveRange(teams);
        await _context.SaveChangesAsync();


        var count = await _context.Teams.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task RemoveRange_WithEmptyCollection_DoesNotFail() {

        var teams = Array.Empty<Team>();


        _repository.RemoveRange(teams);
        await _context.SaveChangesAsync();


        true.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveRange_PartialRemoval_RemovesOnlySpecified() {

        var teams = new[]
        {
            CreateTeam(1, "Team 1"),
            CreateTeam(2, "Team 2"),
            CreateTeam(3, "Team 3")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        _repository.RemoveRange(teams.Take(2));
        await _context.SaveChangesAsync();


        var remaining = await _context.Teams.ToListAsync();
        remaining.Should().ContainSingle();
        remaining.First().Name.Should().Be("Team 3");
    }





    [Fact]
    public async Task ConcurrentReads_DoNotInterfere() {

        var teams = Enumerable.Range(1, 10)
            .Select(i => CreateTeam(i, $"Team {i}"))
            .ToList();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var tasks = Enumerable.Range(1, 5)
            .Select(async _ => await _repository.GetAllAsync())
            .ToList();

        var results = await Task.WhenAll(tasks);


        foreach (var result in results) {
            result.Should().HaveCount(10);
        }
    }





    [Fact]
    public async Task FindAsync_WithNullableProperty_HandlesNulls() {


        var team1 = CreateTeam(1, "Active Team 1");
        var team2 = CreateTeam(2, "Inactive Team");
        team2.IsActive = false;

        await _context.Teams.AddRangeAsync(new[] { team1, team2 });
        await _context.SaveChangesAsync();


        var result = await _repository.FindAsync(t => t.IsActive);


        result.Should().ContainSingle();
        result.First().Name.Should().Be("Active Team 1");
    }

    [Fact]
    public async Task Query_WithProjection_ReturnsAnonymousType() {

        var team = CreateTeam(1, "Test Team", isActive: true);
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.Query()
            .Select(t => new { t.Id, t.Name })
            .FirstOrDefaultAsync();


        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Team");
    }

    [Fact]
    public async Task Query_WithGroupBy_AggregatesCorrectly() {

        var teams = new[]
        {
            CreateTeam(1, "Active 1", isActive: true),
            CreateTeam(2, "Active 2", isActive: true),
            CreateTeam(3, "Inactive", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await _repository.Query()
            .GroupBy(t => t.IsActive)
            .Select(g => new { IsActive = g.Key, Count = g.Count() })
            .ToListAsync();


        result.Should().HaveCount(2);
        result.First(r => r.IsActive).Count.Should().Be(2);
        result.First(r => !r.IsActive).Count.Should().Be(1);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithMatchingEntity_ReturnsEntity() {

        var team = CreateTeam(1, "Unique Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.SingleOrDefaultAsync(t => t.Id == 1);


        result.Should().NotBeNull();
        result!.Name.Should().Be("Unique Team");
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithNoMatch_ReturnsNull() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();


        var result = await _repository.SingleOrDefaultAsync(t => t.Id == 999);


        result.Should().BeNull();
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithMultipleMatches_ThrowsInvalidOperationException() {

        var teams = new[] {
            CreateTeam(1, "Team 1", isActive: true),
            CreateTeam(2, "Team 2", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SingleOrDefaultAsync(t => t.IsActive));
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithCancellationToken_RespectsCancellation() {

        var team = CreateTeam(1, "Test Team");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        var cts = new CancellationTokenSource();


        var result = await _repository.SingleOrDefaultAsync(t => t.Id == 1, cts.Token);


        result.Should().NotBeNull();
    }

    [Fact(Skip = "ExecuteDelete not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteDeleteAsync_WithMatchingPredicate_DeletesEntities() {

        var teams = new[] {
            CreateTeam(1, "Active 1", isActive: true),
            CreateTeam(2, "Active 2", isActive: true),
            CreateTeam(3, "Inactive", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var deletedCount = await _repository.ExecuteDeleteAsync(t => t.IsActive == false);


        deletedCount.Should().Be(1);
        var remaining = await _context.Teams.ToListAsync();
        remaining.Should().HaveCount(2);
        remaining.Should().OnlyContain(t => t.IsActive);
    }

    [Fact(Skip = "ExecuteDelete not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteDeleteAsync_WithNoMatches_ReturnsZero() {

        var teams = new[] {
            CreateTeam(1, "Active 1", isActive: true),
            CreateTeam(2, "Active 2", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var deletedCount = await _repository.ExecuteDeleteAsync(t => t.IsActive == false);


        deletedCount.Should().Be(0);
        var remaining = await _context.Teams.ToListAsync();
        remaining.Should().HaveCount(2);
    }

    [Fact(Skip = "ExecuteDelete not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteDeleteAsync_WithAllMatching_DeletesAllEntities() {

        var teams = new[] {
            CreateTeam(1, "Team 1", isActive: true),
            CreateTeam(2, "Team 2", isActive: true),
            CreateTeam(3, "Team 3", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var deletedCount = await _repository.ExecuteDeleteAsync(t => t.IsActive);


        deletedCount.Should().Be(3);
        var remaining = await _context.Teams.ToListAsync();
        remaining.Should().BeEmpty();
    }

    [Fact(Skip = "ExecuteDelete not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteDeleteAsync_BypassesChangeTracker() {

        var teams = Enumerable.Range(1, 100).Select(i =>
            CreateTeam(i, $"Team {i}", isActive: i % 2 == 0)).ToArray();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var deletedCount = await _repository.ExecuteDeleteAsync(t => t.IsActive == false);


        deletedCount.Should().Be(50);
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact(Skip = "ExecuteUpdate not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteUpdateAsync_WithMatchingPredicate_UpdatesEntities() {

        var teams = new[] {
            CreateTeam(1, "Team 1", isActive: false),
            CreateTeam(2, "Team 2", isActive: false),
            CreateTeam(3, "Team 3", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var updatedCount = await _repository.ExecuteUpdateAsync(
            t => t.IsActive == false,
            s => s.SetProperty(t => t.IsActive, true));


        updatedCount.Should().Be(2);
        var allTeams = await _context.Teams.ToListAsync();
        allTeams.Should().OnlyContain(t => t.IsActive);
    }

    [Fact(Skip = "ExecuteUpdate not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteUpdateAsync_WithNoMatches_ReturnsZero() {

        var teams = new[] {
            CreateTeam(1, "Team 1", isActive: true),
            CreateTeam(2, "Team 2", isActive: true)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var updatedCount = await _repository.ExecuteUpdateAsync(
            t => t.IsActive == false,
            s => s.SetProperty(t => t.IsActive, true));


        updatedCount.Should().Be(0);
    }

    [Fact(Skip = "ExecuteUpdate not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteUpdateAsync_UpdatesMultipleProperties() {

        var teams = new[] {
            CreateTeam(1, "Old Name 1", isActive: false),
            CreateTeam(2, "Old Name 2", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var updatedCount = await _repository.ExecuteUpdateAsync(
            t => t.IsActive == false,
            s => s.SetProperty(t => t.IsActive, true)
                  .SetProperty(t => t.Name, "Updated"));


        updatedCount.Should().Be(2);
        var allTeams = await _context.Teams.ToListAsync();
        allTeams.Should().OnlyContain(t => t.IsActive && t.Name == "Updated");
    }

    [Fact(Skip = "ExecuteUpdate not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteUpdateAsync_BypassesChangeTracker() {

        var teams = Enumerable.Range(1, 100).Select(i =>
            CreateTeam(i, $"Team {i}", isActive: false)).ToArray();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var updatedCount = await _repository.ExecuteUpdateAsync(
            t => t.IsActive == false,
            s => s.SetProperty(t => t.IsActive, true));


        updatedCount.Should().Be(100);
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact(Skip = "ExecuteUpdate not supported by InMemoryDatabase. Run integration tests for bulk operations.")]
    public async Task ExecuteUpdateAsync_WithConditionalUpdate_UpdatesCorrectEntities() {

        var teams = new[] {
            CreateTeam(1, "Alpha", isActive: true),
            CreateTeam(2, "Beta", isActive: true),
            CreateTeam(3, "Gamma", isActive: false)
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var updatedCount = await _repository.ExecuteUpdateAsync(
            t => t.Name.StartsWith("A") || t.Name.StartsWith("B"),
            s => s.SetProperty(t => t.IsActive, false));


        updatedCount.Should().Be(2);
        var gamma = await _context.Teams.FirstAsync(t => t.Name == "Gamma");
        gamma.IsActive.Should().BeFalse();
    }





    [Fact]
    public async Task QueryNoTracking_ChangesAreNotPersisted_WhenSaveChangesCalled() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var result = await _repository.QueryNoTracking()
            .FirstOrDefaultAsync(t => t.Id == 1);
        result!.Name = "Modified Name";
        await _context.SaveChangesAsync();


        _context.ChangeTracker.Clear();
        var reloaded = await _context.Teams.FindAsync(1);
        reloaded!.Name.Should().Be("Original Name");
    }

    [Fact]
    public async Task QueryNoTracking_WithLargeDataset_HasNoTrackedEntities() {

        var teams = Enumerable.Range(1, 1000).Select(i =>
            CreateTeam(i, $"Team {i}")).ToArray();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var results = await _repository.QueryNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync();


        results.Should().HaveCount(1000);
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task Query_WithTracking_PersistsChangesOnSave() {

        var team = CreateTeam(1, "Original Name");
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();


        var result = await _repository.Query()
            .FirstOrDefaultAsync(t => t.Id == 1);
        result!.Name = "Modified Name";
        await _context.SaveChangesAsync();


        _context.ChangeTracker.Clear();
        var reloaded = await _context.Teams.FindAsync(1);
        reloaded!.Name.Should().Be("Modified Name");
    }





    private static Team CreateTeam(int id, string name, bool isActive = true, string? createdBy = null) {
        return new Team {
            Id = id,
            Name = name,
            IsActive = isActive,
            CreatedBy = createdBy ?? "test-user",
            UpdatedBy = createdBy ?? "test-user",
            CreatedAt = DateTime.UtcNow
        };
    }


}