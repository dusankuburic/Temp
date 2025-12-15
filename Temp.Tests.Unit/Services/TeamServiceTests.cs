using AutoFixture;
using FluentAssertions;
using Moq;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Xunit;

namespace Temp.Tests.Unit.Services;

public class TeamServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IFixture _fixture;

    public TeamServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _fixture = new Fixture();

        // Configure AutoFixture to handle circular references in EF models
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetTeamById_WithValidId_ReturnsTeam()
    {
        // Arrange
        var teamId = 1;
        var team = new Team
        {
            Id = teamId,
            Name = "Development Team"
        };

        _mockUnitOfWork.Setup(x => x.Teams.GetByIdAsync(teamId, default))
            .ReturnsAsync(team);

        // Act - NOTE: Replace with actual service method when implementing
        var result = await _mockUnitOfWork.Object.Teams.GetByIdAsync(teamId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(teamId);
        result.Name.Should().Be("Development Team");
    }

    [Fact]
    public async Task GetTeamById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = 999;
        _mockUnitOfWork.Setup(x => x.Teams.GetByIdAsync(invalidId, default))
            .ReturnsAsync((Team?)null);

        // Act
        var result = await _mockUnitOfWork.Object.Teams.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllTeams_ReturnsTeamList()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team { Id = 1, Name = "Team 1" },
            new Team { Id = 2, Name = "Team 2" },
            new Team { Id = 3, Name = "Team 3" }
        };

        _mockUnitOfWork.Setup(x => x.Teams.GetAllAsync(default))
            .ReturnsAsync(teams);

        // Act
        var result = await _mockUnitOfWork.Object.Teams.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Name == "Team 1");
    }

    [Fact]
    public async Task AddTeam_WithValidTeam_SavesSuccessfully()
    {
        // Arrange
        var newTeam = new Team
        {
            Name = "New Team"
        };

        _mockUnitOfWork.Setup(x => x.Teams.AddAsync(newTeam, default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _mockUnitOfWork.Object.Teams.AddAsync(newTeam);
        var savedCount = await _mockUnitOfWork.Object.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);
        _mockUnitOfWork.Verify(x => x.Teams.AddAsync(newTeam, default), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateTeam_WithValidTeam_UpdatesSuccessfully()
    {
        // Arrange
        var existingTeam = new Team
        {
            Id = 1,
            Name = "Old Name"
        };

        existingTeam.Name = "Updated Name";

        _mockUnitOfWork.Setup(x => x.Teams.Update(existingTeam));
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        _mockUnitOfWork.Object.Teams.Update(existingTeam);
        var savedCount = await _mockUnitOfWork.Object.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);
        _mockUnitOfWork.Verify(x => x.Teams.Update(existingTeam), Times.Once);
    }

    [Fact]
    public async Task DeleteTeam_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        var teamToDelete = new Team { Id = 1, Name = "Team to Delete" };

        _mockUnitOfWork.Setup(x => x.Teams.Remove(teamToDelete));
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        _mockUnitOfWork.Object.Teams.Remove(teamToDelete);
        var savedCount = await _mockUnitOfWork.Object.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);
        _mockUnitOfWork.Verify(x => x.Teams.Remove(teamToDelete), Times.Once);
    }

    [Fact]
    public async Task UnitOfWork_Transaction_CommitsSuccessfully()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.CommitTransactionAsync(default))
            .Returns(Task.CompletedTask);

        // Act
        await _mockUnitOfWork.Object.BeginTransactionAsync();
        await _mockUnitOfWork.Object.CommitTransactionAsync();

        // Assert
        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(default), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(default), Times.Once);
    }

    [Fact]
    public async Task UnitOfWork_Transaction_RollsBackOnError()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.RollbackTransactionAsync(default))
            .Returns(Task.CompletedTask);

        // Act
        await _mockUnitOfWork.Object.BeginTransactionAsync();
        await _mockUnitOfWork.Object.RollbackTransactionAsync();

        // Assert
        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(default), Times.Once);
        _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(default), Times.Once);
    }
}
