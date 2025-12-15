using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Database.Repositories;
using Temp.Domain.Models;
using Xunit;

namespace Temp.Tests.Unit.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<Team> _repository;

    public RepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<Team>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "Test Team" };
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Team");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Team 1" },
            new Team { Id = 2, Name = "Team 2" },
            new Team { Id = 3, Name = "Team 3" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Name == "Team 1");
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ReturnsMatchingEntities()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Dev Team" },
            new Team { Id = 2, Name = "QA Team" },
            new Team { Id = 3, Name = "Dev Ops Team" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(t => t.Name.Contains("Dev"));

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Name == "Dev Team");
        result.Should().Contain(t => t.Name == "Dev Ops Team");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMatchingPredicate_ReturnsFirst()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Team A" },
            new Team { Id = 2, Name = "Team B" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(t => t.Name == "Team A");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Team A");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNonMatchingPredicate_ReturnsNull()
    {
        // Act
        var result = await _repository.FirstOrDefaultAsync(t => t.Name == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AnyAsync_WithMatchingPredicate_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "Test Team" };
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AnyAsync(t => t.Name == "Test Team");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithNonMatchingPredicate_ReturnsFalse()
    {
        // Act
        var result = await _repository.AnyAsync(t => t.Name == "NonExistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ReturnsTotal()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Team 1" },
            new Team { Id = 2, Name = "Team 2" },
            new Team { Id = 3, Name = "Team 3" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsMatchingCount()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Dev Team" },
            new Team { Id = 2, Name = "QA Team" },
            new Team { Id = 3, Name = "Dev Ops" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync(t => t.Name.Contains("Dev"));

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToDatabase()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "New Team" };

        // Act
        await _repository.AddAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.Teams.FindAsync(1);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Team");
    }

    [Fact]
    public async Task AddRangeAsync_AddsMultipleEntities()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Team 1" },
            new Team { Id = 2, Name = "Team 2" }
        };

        // Act
        await _repository.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.Teams.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task Update_ModifiesExistingEntity()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "Original Name" };
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        team.Name = "Updated Name";
        _repository.Update(team);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Teams.FindAsync(1);
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Remove_DeletesEntity()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "Team to Delete" };
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(team);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Teams.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task RemoveRange_DeletesMultipleEntities()
    {
        // Arrange
        var teams = new[]
        {
            new Team { Id = 1, Name = "Team 1" },
            new Team { Id = 2, Name = "Team 2" }
        };

        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();

        // Act
        _repository.RemoveRange(teams);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.Teams.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public void Query_ReturnsQueryable()
    {
        // Act
        var query = _repository.Query();

        // Assert
        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQueryable<Team>>();
    }

    [Fact]
    public void QueryNoTracking_ReturnsQueryableWithNoTracking()
    {
        // Act
        var query = _repository.QueryNoTracking();

        // Assert
        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQueryable<Team>>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
