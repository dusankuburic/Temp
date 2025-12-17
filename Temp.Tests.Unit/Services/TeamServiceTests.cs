using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Teams;
using Temp.Services.Teams.Exceptions;

namespace Temp.Tests.Unit.Services;

public class TeamServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Team>> _mockTeamRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly ITeamService _service;

    public TeamServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeamRepository = new Mock<IRepository<Team>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        _mockUnitOfWork.Setup(uow => uow.Teams).Returns(_mockTeamRepository.Object);

        _service = new TeamService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task DeleteTeamAsync_WithValidId_DeletesTeam() {
        var teamId = 1;
        var team = _fixture.Build<Team>().With(t => t.Id, teamId).Create();

        _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId, default)).ReturnsAsync(team);
        _mockTeamRepository.Setup(r => r.Remove(It.IsAny<Team>()));
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        await _service.DeleteTeamAsync(teamId);

        _mockTeamRepository.Verify(r => r.Remove(team), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteTeamAsync_WithNonExistentId_ThrowsTeamNotFoundException() {
        var nonExistentId = 999;
        _mockTeamRepository.Setup(r => r.GetByIdAsync(nonExistentId, default)).ReturnsAsync((Team?)null);

        await Assert.ThrowsAsync<TeamServiceException>(async () => await _service.DeleteTeamAsync(nonExistentId));
    }

    [Fact]
    public async Task TeamExists_WithExistingName_ReturnsTrue() {
        var teamName = "Existing Team";
        var groupId = 1;

        _mockTeamRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Team, bool>>>(), default)).ReturnsAsync(true);

        var result = await _service.TeamExists(teamName, groupId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllTeamsAsync_ReturnsAllTeams() {
        var teams = _fixture.Build<Team>().CreateMany(5).ToList();
        _mockTeamRepository.Setup(r => r.QueryNoTracking()).Returns(teams.AsQueryable().BuildMock());

        var result = await _service.GetAllTeamsAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }
}
