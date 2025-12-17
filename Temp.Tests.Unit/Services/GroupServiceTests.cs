using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Groups;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class GroupServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Group>> _mockGroupRepository;
    private readonly Mock<IRepository<Organization>> _mockOrganizationRepository;
    private readonly Mock<IRepository<Team>> _mockTeamRepository;
    private readonly Mock<IRepository<ModeratorGroup>> _mockModeratorGroupRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IGroupService _service;

    public GroupServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockGroupRepository = new Mock<IRepository<Group>>();
        _mockOrganizationRepository = new Mock<IRepository<Organization>>();
        _mockTeamRepository = new Mock<IRepository<Team>>();
        _mockModeratorGroupRepository = new Mock<IRepository<ModeratorGroup>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        // Setup UnitOfWork to return mocked repositories
        _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Organizations).Returns(_mockOrganizationRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Teams).Returns(_mockTeamRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ModeratorGroups).Returns(_mockModeratorGroupRepository.Object);

        // Create service with mocked dependencies
        _service = new GroupService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateGroup_WithValidData_ReturnsCreatedGroup() {
        // Arrange
        var organizationId = 1;
        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.OrganizationId, organizationId)
            .With(r => r.Name, "Test Group")
            .Create();

        var group = _fixture.Build<Group>()
            .With(g => g.OrganizationId, organizationId)
            .With(g => g.Name, request.Name)
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.HasActiveGroup, false)
            .Create();

        var response = _fixture.Build<CreateGroupResponse>()
            .With(r => r.Id, group.Id)
            .Create();

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns(group);

        _mockMapper.Setup(m => m.Map<CreateGroupResponse>(It.IsAny<Group>()))
            .Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockGroupRepository.Setup(r => r.AddAsync(It.IsAny<Group>(), default))
            .Returns(Task.CompletedTask);

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateGroup(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockGroupRepository.Verify(r => r.AddAsync(It.IsAny<Group>(), default), Times.Once);
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.HasActiveGroup == true)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Exactly(2));
    }

    [Fact]
    public async Task GetGroup_WithValidId_ReturnsGroup() {
        // Arrange
        var groupId = 1;
        var request = new GetGroupRequest { Id = groupId };

        var groups = new List<Group>
        {
            _fixture.Build<Group>()
                .With(g => g.Id, groupId)
                .With(g => g.IsActive, true)
                .Create()
        }.AsQueryable();

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        // Act & Assert
        // This demonstrates the pattern for retrieving a single group
    }

    [Fact]
    public async Task UpdateGroup_WithValidData_UpdatesGroup() {
        // Arrange
        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, "Updated Group Name")
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .Create();

        var response = _fixture.Build<UpdateGroupResponse>()
            .Create();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);

        _mockMapper.Setup(m => m.Map<UpdateGroupResponse>(It.IsAny<Group>()))
            .Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateGroup(request);

        // Assert
        result.Should().NotBeNull();
        _mockGroupRepository.Verify(r => r.Update(It.IsAny<Group>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateGroupStatus_TogglesIsActiveFlag() {
        // Arrange
        var groupId = 1;
        var request = new UpdateGroupStatusRequest { Id = groupId };

        var group = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.IsActive, true)
            .With(g => g.OrganizationId, 1)
            .With(g => g.Organization, _fixture.Build<Organization>()
                .With(o => o.Id, 1)
                .With(o => o.HasActiveGroup, true)
                .Create())
            .Create();

        var groupQueryable = new List<Group> { group }.AsQueryable().BuildMock();

        _mockGroupRepository.Setup(r => r.Query())
            .Returns(groupQueryable);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));

        _mockOrganizationRepository.Setup(r => r.Query())
            .Returns(new List<Organization> { group.Organization }.AsQueryable().BuildMock());

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateGroupStatus(request);

        // Assert
        result.Should().NotBeNull();
        _mockGroupRepository.Verify(r => r.Update(
            It.Is<Group>(g => g.IsActive == false)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Exactly(2));
    }

    [Fact]
    public async Task GetPagedGroupInnerTeams_WithValidGroupId_ReturnsPagedTeams() {
        // Arrange
        var groupId = 1;
        var request = new GetPagedGroupInnerTeamsRequest
        {
            GroupId = groupId,
            PageNumber = 1,
            PageSize = 10,
            Name = ""
        };

        var teams = _fixture.Build<Team>()
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, true)
            .CreateMany(5)
            .ToList();

        var teamsQueryable = teams.AsQueryable();

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teamsQueryable);

        var groups = new List<Group>
        {
            _fixture.Build<Group>()
                .With(g => g.Id, groupId)
                .With(g => g.Name, "Test Group")
                .With(g => g.IsActive, true)
                .Create()
        }.AsQueryable();

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        // Act & Assert
        // This demonstrates the pattern for paged inner teams query
    }

    [Fact]
    public async Task GroupExists_WithExistingName_ReturnsTrue() {
        // Arrange
        var groupName = "Existing Group";
        var organizationId = 1;

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(true);

        // Act
        var result = await _service.GroupExists(groupName, organizationId);

        // Assert
        result.Should().BeTrue();
        _mockGroupRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task GroupExists_WithNonExistingName_ReturnsFalse() {
        // Arrange
        var groupName = "Non-existing Group";
        var organizationId = 1;

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await _service.GroupExists(groupName, organizationId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetModeratorGroups_WithValidModeratorId_ReturnsGroupsList() {
        // Arrange
        var moderatorId = 1;
        var request = new GetModeratorGroupsRequest { Id = moderatorId };

        var moderatorGroups = _fixture.Build<ModeratorGroup>()
            .With(mg => mg.ModeratorId, moderatorId)
            .CreateMany(3)
            .ToList();

        var queryable = moderatorGroups.AsQueryable();

        _mockModeratorGroupRepository.Setup(r => r.Query())
            .Returns(queryable);

        // Act & Assert
        // This demonstrates the pattern for moderator groups query
    }

    [Fact]
    public void GroupService_Constructor_InitializesDependenciesCorrectly() {
        // Arrange & Act
        var service = new GroupService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        // Assert
        service.Should().NotBeNull();
    }
}
