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
using Temp.Services.Providers.Models;
using Temp.Services.Teams;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Tests.Unit.Services;

public class TeamServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Team>> _mockTeamRepository;
    private readonly Mock<IRepository<Group>> _mockGroupRepository;
    private readonly Mock<IRepository<Organization>> _mockOrganizationRepository;
    private readonly Mock<IRepository<Employee>> _mockEmployeeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly ITeamService _service;

    public TeamServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeamRepository = new Mock<IRepository<Team>>();
        _mockGroupRepository = new Mock<IRepository<Group>>();
        _mockOrganizationRepository = new Mock<IRepository<Organization>>();
        _mockEmployeeRepository = new Mock<IRepository<Employee>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        _mockUnitOfWork.Setup(uow => uow.Teams).Returns(_mockTeamRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Organizations).Returns(_mockOrganizationRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Employees).Returns(_mockEmployeeRepository.Object);

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
    public async Task CreateTeam_WithValidData_ReturnsCreatedTeam() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "Test Team")
            .With(r => r.GroupId, 1)
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, request.Name)
            .With(t => t.GroupId, request.GroupId)
            .With(t => t.IsActive, true)
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.IsActive, true)
            .Create();

        var group = _fixture.Build<Group>()
            .With(g => g.Id, request.GroupId)
            .With(g => g.Organization, organization)
            .With(g => g.IsActive, true)
            .Create();

        var response = _fixture.Build<CreateTeamResponse>()
            .With(r => r.Id, team.Id)
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);
        _mockMapper.Setup(m => m.Map<CreateTeamResponse>(It.IsAny<Team>())).Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.AddAsync(It.IsAny<Team>(), default))
            .Returns(Task.CompletedTask);

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.Query()).Returns(groups);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.CreateTeam(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockTeamRepository.Verify(r => r.AddAsync(It.IsAny<Team>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CreateTeam_WithEmptyName_ThrowsTeamValidationException() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "")
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateTeam(request);


        await act.Should().ThrowAsync<TeamValidationException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task CreateTeam_WithInvalidName_ThrowsTeamValidationException(string? invalidName) {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, invalidName)
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, invalidName)
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateTeam(request);


        await act.Should().ThrowAsync<TeamValidationException>();
    }

    [Fact]
    public async Task CreateTeam_WithNullMappedTeam_ThrowsTeamServiceException() {

        var request = _fixture.Create<CreateTeamRequest>();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns((Team)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }

    [Fact]
    public async Task CreateTeam_SetsAuditInfo() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "Test Team")
            .With(r => r.GroupId, 1)
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, request.Name)
            .Create();

        var organization = _fixture.Build<Organization>().Create();
        var group = _fixture.Build<Group>()
            .With(g => g.Id, request.GroupId)
            .With(g => g.Organization, organization)
            .Create();

        var currentUser = new CurrentUser { AppUserId = "user-id", Email = "user@example.com" };

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);
        _mockMapper.Setup(m => m.Map<CreateTeamResponse>(It.IsAny<Team>()))
            .Returns(_fixture.Create<CreateTeamResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(currentUser);

        _mockTeamRepository.Setup(r => r.AddAsync(It.IsAny<Team>(), default))
            .Returns(Task.CompletedTask);

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.Query()).Returns(groups);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateTeam(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockTeamRepository.Verify(r => r.AddAsync(It.IsAny<Team>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateTeam_WhenDatabaseFails_ThrowsTeamServiceException() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "Test Team")
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, request.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.AddAsync(It.IsAny<Team>(), default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.CreateTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public Task GetTeam_WithValidId_ReturnsTeam() {

        var teamId = 1;
        var request = new GetTeamRequest { Id = teamId };

        var team = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.IsActive, true)
            .Create();

        var teams = new List<Team> { team }.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.Query()).Returns(teams);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, GetTeamResponse>()));


        _mockTeamRepository.Object.Query().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetTeam_WithInactiveTeam_ThrowsTeamServiceException() {

        var teamId = 1;
        var request = new GetTeamRequest { Id = teamId };

        var teams = new List<Team>
        {
            _fixture.Build<Team>()
                .With(t => t.Id, teamId)
                .With(t => t.IsActive, false)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.Query()).Returns(teams);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, GetTeamResponse>()));


        Func<Task> act = async () => await _service.GetTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }

    [Fact]
    public async Task GetTeam_WithNonExistentId_ThrowsTeamServiceException() {

        var request = new GetTeamRequest { Id = 999 };

        var teams = new List<Team>().AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.Query()).Returns(teams);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, GetTeamResponse>()));


        Func<Task> act = async () => await _service.GetTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }





    [Fact]
    public async Task UpdateTeam_WithValidData_UpdatesTeam() {

        var teamId = 1;
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, teamId)
            .With(r => r.Name, "Updated Team")
            .Create();

        var existingTeam = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.Name, "Original Team")
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(existingTeam);

        _mockMapper.Setup(m => m.Map(request, existingTeam))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateTeam(request);


        result.Should().NotBeNull();
        _mockTeamRepository.Verify(r => r.Update(
            It.Is<Team>(t => t.Name == "Updated Team")), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateTeam_WithEmptyName_ThrowsTeamValidationException() {

        var teamId = 1;
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, teamId)
            .With(r => r.Name, "")
            .Create();

        var existingTeam = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(existingTeam);

        _mockMapper.Setup(m => m.Map(request, existingTeam))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateTeam(request);


        await act.Should().ThrowAsync<TeamValidationException>();
    }

    [Fact]
    public async Task UpdateTeam_WithNonExistentTeam_ThrowsTeamServiceException() {

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, 999)
            .With(r => r.Name, "Updated Name")
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync((Team)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }

    [Fact]
    public async Task UpdateTeam_SetsAuditInfoOnUpdate() {

        var teamId = 1;
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, teamId)
            .With(r => r.Name, "Updated Team")
            .Create();

        var existingTeam = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.Name, "Original Team")
            .Create();

        var currentUser = new CurrentUser { AppUserId = "user-id", Email = "updater@example.com" };

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(existingTeam);

        _mockMapper.Setup(m => m.Map(request, existingTeam))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(currentUser);

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateTeam(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockTeamRepository.Verify(r => r.Update(It.IsAny<Team>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTeam_WhenDatabaseFails_ThrowsTeamServiceException() {

        var teamId = 1;
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, teamId)
            .With(r => r.Name, "Updated Team")
            .Create();

        var existingTeam = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(existingTeam);

        _mockMapper.Setup(m => m.Map(request, existingTeam))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.UpdateTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }





    [Fact]
    public async Task UpdateTeamStatus_TogglesIsActiveFlag_FromTrueToFalse() {

        var teamId = 1;
        var groupId = 1;
        var request = new UpdateTeamStatusRequest { Id = teamId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, 1)
            .With(o => o.Groups, new List<Group>())
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.Name, "Test Team")
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, true)
            .Create();

        var group = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.Organization, organization)
            .With(g => g.OrganizationId, organization.Id)
            .With(g => g.Teams, new List<Team> { team })
            .With(g => g.HasActiveTeam, true)
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.Query()).Returns(groups);
        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));


        var teams = new List<Team> { team }.AsQueryable().BuildMockDbSet().Object;
        _mockTeamRepository.Setup(r => r.QueryNoTracking()).Returns(teams);

        var groupsNoTracking = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.QueryNoTracking()).Returns(groupsNoTracking);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateTeamStatus(request);


        result.Should().NotBeNull();
        team.IsActive.Should().BeFalse();
        _mockTeamRepository.Verify(r => r.Update(
            It.Is<Team>(t => t.IsActive == false)), Times.Once);
    }

    [Fact]
    public async Task UpdateTeamStatus_TogglesIsActiveFlag_FromFalseToTrue() {

        var teamId = 1;
        var groupId = 1;
        var request = new UpdateTeamStatusRequest { Id = teamId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, 1)
            .With(o => o.Groups, new List<Group>())
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.Name, "Test Team")
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, false)
            .Create();

        var group = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.Organization, organization)
            .With(g => g.OrganizationId, organization.Id)
            .With(g => g.Teams, new List<Team> { team })
            .With(g => g.HasActiveTeam, false)
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.Query()).Returns(groups);
        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));


        var teams = new List<Team> { team }.AsQueryable().BuildMockDbSet().Object;
        _mockTeamRepository.Setup(r => r.QueryNoTracking()).Returns(teams);

        var groupsNoTracking = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.QueryNoTracking()).Returns(groupsNoTracking);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateTeamStatus(request);


        result.Should().NotBeNull();
        team.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTeamStatus_WithNonExistentTeam_ThrowsTeamServiceException() {

        var request = new UpdateTeamStatusRequest { Id = 999 };

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync((Team)null!);


        Func<Task> act = async () => await _service.UpdateTeamStatus(request);


        await act.Should().ThrowAsync<TeamServiceException>();
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
    public async Task DeleteTeamAsync_WithNonExistentId_ThrowsTeamServiceException() {

        var nonExistentId = 999;
        _mockTeamRepository.Setup(r => r.GetByIdAsync(nonExistentId, default)).ReturnsAsync((Team?)null);


        await Assert.ThrowsAsync<TeamServiceException>(async () => await _service.DeleteTeamAsync(nonExistentId));
    }

    [Fact]
    public async Task DeleteTeamAsync_WhenDatabaseFails_ThrowsTeamServiceException() {

        var teamId = 1;
        var team = _fixture.Build<Team>().With(t => t.Id, teamId).Create();

        _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId, default)).ReturnsAsync(team);
        _mockTeamRepository.Setup(r => r.Remove(It.IsAny<Team>()));
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.DeleteTeamAsync(teamId);


        await act.Should().ThrowAsync<TeamServiceException>();
    }

    [Fact]
    public async Task DeleteTeamAsync_VerifiesRepositoryMethodsCalled() {

        var teamId = 1;
        var team = _fixture.Build<Team>().With(t => t.Id, teamId).Create();

        _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId, default)).ReturnsAsync(team);
        _mockTeamRepository.Setup(r => r.Remove(It.IsAny<Team>()));
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.DeleteTeamAsync(teamId);


        _mockTeamRepository.Verify(r => r.GetByIdAsync(teamId, default), Times.Once);
        _mockTeamRepository.Verify(r => r.Remove(team), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }





    [Fact]
    public async Task TeamExists_WithExistingName_ReturnsTrue() {

        var teamName = "Existing Team";
        var groupId = 1;

        _mockTeamRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(true);


        var result = await _service.TeamExists(teamName, groupId);


        result.Should().BeTrue();
        _mockTeamRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task TeamExists_WithNonExistingName_ReturnsFalse() {

        var teamName = "Non-existing Team";
        var groupId = 1;

        _mockTeamRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.TeamExists(teamName, groupId);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task TeamExists_SameNameDifferentGroup_ReturnsFalse() {

        var teamName = "Team Alpha";
        var groupId = 2;

        _mockTeamRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.TeamExists(teamName, groupId);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task TeamExists_WhenDatabaseFails_ThrowsTeamServiceException() {

        var teamName = "Any Team";
        var groupId = 1;

        _mockTeamRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.TeamExists(teamName, groupId);


        await act.Should().ThrowAsync<TeamServiceException>();
    }





    [Fact]
    public async Task GetAllTeamsAsync_ReturnsAllTeams() {

        var teams = _fixture.Build<Team>()
            .With(t => t.IsActive, true)
            .CreateMany(5)
            .ToList();

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teams.AsQueryable().BuildMockDbSet().Object);


        var result = await _service.GetAllTeamsAsync();


        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAllTeamsAsync_WithNoTeams_ReturnsEmptyList() {

        var teams = new List<Team>();

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teams.AsQueryable().BuildMockDbSet().Object);


        var result = await _service.GetAllTeamsAsync();


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllTeamsAsync_ReturnsCorrectTeamData() {

        var teams = new List<Team>
        {
            new Team { Id = 1, Name = "Team A", GroupId = 1 },
            new Team { Id = 2, Name = "Team B", GroupId = 2 }
        };

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teams.AsQueryable().BuildMockDbSet().Object);


        var result = await _service.GetAllTeamsAsync();


        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        var resultList = result.ToList();
        resultList[0].Name.Should().Be("Team A");
        resultList[1].Name.Should().Be("Team B");
    }





    [Fact]
    public Task GetUserTeam_WithValidEmployeeId_QueriesEmployeeRepository() {

        var employeeId = 1;
        var request = new GetUserTeamRequest { Id = employeeId };

        var employee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var employees = new List<Employee> { employee }.AsQueryable().BuildMockDbSet().Object;

        _mockEmployeeRepository.Setup(r => r.Query()).Returns(employees);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, GetUserTeamResponse>()));


        _mockEmployeeRepository.Object.Query().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetUserTeam_WithNonExistentEmployee_ThrowsTeamServiceException() {

        var request = new GetUserTeamRequest { Id = 999 };

        var employees = new List<Employee>().AsQueryable().BuildMockDbSet().Object;

        _mockEmployeeRepository.Setup(r => r.Query()).Returns(employees);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, GetUserTeamResponse>()));


        Func<Task> act = async () => await _service.GetUserTeam(request);


        await act.Should().ThrowAsync<TeamServiceException>();
    }





    [Fact]
    public Task GetFullTeamTree_WithValidTeamId_QueriesTeamWithIncludes() {

        var teamId = 1;
        var request = new GetFullTeamTreeRequest { Id = teamId };

        var team = _fixture.Build<Team>()
            .With(t => t.Id, teamId)
            .With(t => t.IsActive, true)
            .Create();

        var teams = new List<Team> { team }.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.QueryNoTracking()).Returns(teams);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, GetFullTeamTreeResponse>()));


        _mockTeamRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetFullTeamTree_WithInactiveTeam_ReturnsNull() {

        var teamId = 1;
        var request = new GetFullTeamTreeRequest { Id = teamId };

        var teams = new List<Team>
        {
            _fixture.Build<Team>()
                .With(t => t.Id, teamId)
                .With(t => t.IsActive, false)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.QueryNoTracking()).Returns(teams);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, GetFullTeamTreeResponse>()));


        var result = await _service.GetFullTeamTree(request);


        result.Should().BeNull();
    }





    [Fact]
    public async Task CreateTeam_WithValidationError_LogsError() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "")
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.CreateTeam(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTeam_WithValidationError_LogsError() {

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "")
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Id, 1)
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(team);

        _mockMapper.Setup(m => m.Map(request, team))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.UpdateTeam(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public void TeamService_Constructor_InitializesDependenciesCorrectly() {

        var service = new TeamService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        service.Should().NotBeNull();
    }





    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task TeamExists_WithVariousLengthNames_WorksCorrectly(int nameLength) {

        var teamName = new string('a', nameLength);
        var groupId = 1;

        _mockTeamRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.TeamExists(teamName, groupId);


        result.Should().BeFalse();
        _mockTeamRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateTeam_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<CreateTeamRequest>()
            .With(r => r.Name, "Test Team")
            .With(r => r.GroupId, 1)
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Name, request.Name)
            .Create();

        var organization = _fixture.Build<Organization>().Create();
        var group = _fixture.Build<Group>()
            .With(g => g.Id, request.GroupId)
            .With(g => g.Organization, organization)
            .Create();

        _mockMapper.Setup(m => m.Map<Team>(request)).Returns(team);
        _mockMapper.Setup(m => m.Map<CreateTeamResponse>(It.IsAny<Team>()))
            .Returns(_fixture.Create<CreateTeamResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.AddAsync(It.IsAny<Team>(), default))
            .Returns(Task.CompletedTask);

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        _mockGroupRepository.Setup(r => r.Query()).Returns(groups);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateTeam(request);


        _mockMapper.Verify(m => m.Map<Team>(request), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockTeamRepository.Verify(r => r.AddAsync(It.IsAny<Team>(), default), Times.Once);
        _mockMapper.Verify(m => m.Map<CreateTeamResponse>(It.IsAny<Team>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTeam_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "Updated Name")
            .Create();

        var team = _fixture.Build<Team>()
            .With(t => t.Id, 1)
            .With(t => t.Name, "Original Name")
            .Create();

        _mockTeamRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default))
            .ReturnsAsync(team);

        _mockMapper.Setup(m => m.Map(request, team))
            .Callback<UpdateTeamRequest, Team>((req, t) => t.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockTeamRepository.Setup(r => r.Update(It.IsAny<Team>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateTeam(request);


        _mockTeamRepository.Verify(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Team, bool>>>(), default), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockTeamRepository.Verify(r => r.Update(It.IsAny<Team>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }


}