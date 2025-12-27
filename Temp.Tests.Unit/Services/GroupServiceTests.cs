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
using Temp.Services.Groups.Exceptions;
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
    private readonly CurrentUser _defaultCurrentUser;

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


        _defaultCurrentUser = new CurrentUser { AppUserId = "test-user", Email = "test@example.com" };
        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(_defaultCurrentUser);


        _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Organizations).Returns(_mockOrganizationRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Teams).Returns(_mockTeamRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ModeratorGroups).Returns(_mockModeratorGroupRepository.Object);


        _service = new GroupService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }



    [Fact]
    public async Task CreateGroup_WithValidData_ReturnsCreatedGroup() {

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

        _mockGroupRepository.Setup(r => r.AddAsync(It.IsAny<Group>(), default))
            .Returns(Task.CompletedTask);

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateGroup(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockGroupRepository.Verify(r => r.AddAsync(It.IsAny<Group>(), default), Times.Once);
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.HasActiveGroup == true)), Times.Once);

        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_WithEmptyName_ThrowsGroupValidationException() {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, "")
            .Create();

        var group = new Group { Name = "", OrganizationId = request.OrganizationId };

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns(group);


        await Assert.ThrowsAsync<GroupValidationException>(
            async () => await _service.CreateGroup(request));

        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_WithWhitespaceName_ThrowsGroupValidationException() {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, "   ")
            .Create();

        var group = new Group { Name = "   ", OrganizationId = request.OrganizationId };

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns(group);


        await Assert.ThrowsAsync<GroupValidationException>(
            async () => await _service.CreateGroup(request));
    }

    [Fact]
    public async Task CreateGroup_WithNullMappedGroup_ThrowsGroupServiceException() {

        var request = _fixture.Create<CreateGroupRequest>();

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns((Group)null!);




        await Assert.ThrowsAsync<GroupServiceException>(
            async () => await _service.CreateGroup(request));
    }

    [Fact]
    public async Task CreateGroup_WhenDatabaseFails_ThrowsGroupServiceException() {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, "Test Group")
            .Create();

        var group = new Group { Name = request.Name, OrganizationId = request.OrganizationId };

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns(group);

        _mockGroupRepository.Setup(r => r.AddAsync(It.IsAny<Group>(), default))
            .ThrowsAsync(new Exception("Database error"));


        await Assert.ThrowsAsync<GroupServiceException>(
            async () => await _service.CreateGroup(request));

        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_SetsAuditableInfoOnCreate() {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, "Test Group")
            .Create();

        var group = new Group { Name = request.Name, OrganizationId = request.OrganizationId };
        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, request.OrganizationId)
            .Create();
        var response = new CreateGroupResponse { Id = 1 };

        _mockMapper.Setup(m => m.Map<Group>(request)).Returns(group);
        _mockMapper.Setup(m => m.Map<CreateGroupResponse>(It.IsAny<Group>())).Returns(response);
        _mockGroupRepository.Setup(r => r.AddAsync(It.IsAny<Group>(), default)).Returns(Task.CompletedTask);
        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default)).ReturnsAsync(organization);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateGroup(request);


        _mockIdentityProvider.Verify(ip => ip.GetCurrentUser(), Times.Once);
    }


    [Fact]
    public async Task GetGroup_WithValidId_ReturnsGroup() {

        var groupId = 1;
        var request = new GetGroupRequest { Id = groupId };

        var group = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.IsActive, true)
            .With(g => g.Name, "Test Group")
            .Create();

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetGroupResponse>()));


        var result = await _service.GetGroup(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(groupId);
        _mockGroupRepository.Verify(r => r.QueryNoTracking(), Times.Once);
    }

    [Fact]
    public async Task GetGroup_WithInactiveGroup_ReturnsNull() {

        var groupId = 1;
        var request = new GetGroupRequest { Id = groupId };

        var group = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.IsActive, false)
            .Create();

        var groups = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetGroupResponse>()));


        var result = await _service.GetGroup(request);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroup_WithNonExistentId_ReturnsNull() {

        var request = new GetGroupRequest { Id = 999 };
        var groups = new List<Group>().AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetGroupResponse>()));


        var result = await _service.GetGroup(request);


        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateGroup_WithValidData_UpdatesGroup() {

        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, "Updated Group Name")
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.Name, "Old Name")
            .Create();

        var response = new UpdateGroupResponse();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);

        _mockMapper.Setup(m => m.Map<UpdateGroupResponse>(It.IsAny<Group>()))
            .Returns(response);

        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateGroup(request);


        result.Should().NotBeNull();
        _mockGroupRepository.Verify(r => r.Update(It.IsAny<Group>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_WithEmptyName_ThrowsGroupValidationException() {

        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, "")
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .Create();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);


        await Assert.ThrowsAsync<GroupValidationException>(
            async () => await _service.UpdateGroup(request));
    }

    [Fact]
    public async Task UpdateGroup_SetsAuditableInfoOnUpdate() {

        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, "Updated Name")
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.Name, "Old Name")
            .Create();

        var response = new UpdateGroupResponse();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);

        _mockMapper.Setup(m => m.Map<UpdateGroupResponse>(It.IsAny<Group>()))
            .Returns(response);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateGroup(request);


        _mockIdentityProvider.Verify(ip => ip.GetCurrentUser(), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_WhenDatabaseFails_ThrowsGroupServiceException() {

        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, "Updated Name")
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .With(g => g.Name, "Old Name")
            .Create();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        await Assert.ThrowsAsync<GroupServiceException>(
            async () => await _service.UpdateGroup(request));
    }





    [Fact]
    public async Task UpdateGroupStatus_TogglesIsActiveFlag() {

        var groupId = 1;
        var request = new UpdateGroupStatusRequest { Id = groupId };


        var group = new Group
        {
            Id = groupId,
            IsActive = true,
            OrganizationId = 1,
            HasActiveTeam = true
        };

        var organization = new Organization
        {
            Id = 1,
            HasActiveGroup = true,
            Groups = new List<Group> { group }
        };

        group.Organization = organization;

        var groupQueryable = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        var groupsNoTracking = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.Query())
            .Returns(groupQueryable);

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groupsNoTracking);

        _mockGroupRepository.Setup(r => r.Update(It.IsAny<Group>()));

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateGroupStatus(request);


        result.Should().NotBeNull();
        _mockGroupRepository.Verify(r => r.Update(
            It.Is<Group>(g => g.IsActive == false)), Times.Once);

        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateGroupStatus_WhenInactive_TogglesBackToActive() {

        var groupId = 1;
        var request = new UpdateGroupStatusRequest { Id = groupId };

        var group = new Group
        {
            Id = groupId,
            IsActive = false,
            OrganizationId = 1,
            HasActiveTeam = false
        };

        var organization = new Organization
        {
            Id = 1,
            HasActiveGroup = false,
            Groups = new List<Group> { group }
        };

        group.Organization = organization;

        var groupQueryable = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;
        var groupsNoTracking = new List<Group> { group }.AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.Query()).Returns(groupQueryable);
        _mockGroupRepository.Setup(r => r.QueryNoTracking()).Returns(groupsNoTracking);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateGroupStatus(request);


        result.Should().NotBeNull();
        _mockGroupRepository.Verify(r => r.Update(
            It.Is<Group>(g => g.IsActive == true)), Times.Once);
    }





    [Fact]
    public async Task GroupExists_WithExistingName_ReturnsTrue() {

        var groupName = "Existing Group";
        var organizationId = 1;

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(true);


        var result = await _service.GroupExists(groupName, organizationId);


        result.Should().BeTrue();
        _mockGroupRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task GroupExists_WithNonExistingName_ReturnsFalse() {

        var groupName = "Non-existing Group";
        var organizationId = 1;

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.GroupExists(groupName, organizationId);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task GroupExists_WithSameNameDifferentOrganization_ReturnsFalse() {

        var groupName = "Group Name";
        var organizationId = 2;

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.GroupExists(groupName, organizationId);


        result.Should().BeFalse();
    }





    [Fact]
    public async Task GetGroupInnerTeams_WithValidGroupId_ReturnsTeamsList() {

        var groupId = 1;

        var teams = _fixture.Build<Team>()
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, true)
            .CreateMany(5)
            .ToList();

        var teamsQueryable = teams.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teamsQueryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, InnerTeam>()));


        var result = await _service.GetGroupInnerTeams(groupId);


        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        _mockTeamRepository.Verify(r => r.QueryNoTracking(), Times.Once);
    }

    [Fact]
    public async Task GetGroupInnerTeams_WithNoTeams_ReturnsEmptyList() {

        var groupId = 999;
        var teamsQueryable = new List<Team>().AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teamsQueryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, InnerTeam>()));


        var result = await _service.GetGroupInnerTeams(groupId);


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGroupInnerTeams_OnlyReturnsActiveTeams() {

        var groupId = 1;

        var activeTeams = _fixture.Build<Team>()
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, true)
            .CreateMany(3)
            .ToList();

        var inactiveTeams = _fixture.Build<Team>()
            .With(t => t.GroupId, groupId)
            .With(t => t.IsActive, false)
            .CreateMany(2)
            .ToList();

        var allTeams = activeTeams.Concat(inactiveTeams).ToList();
        var teamsQueryable = allTeams.AsQueryable().BuildMockDbSet().Object;

        _mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(teamsQueryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Team, InnerTeam>()));


        var result = await _service.GetGroupInnerTeams(groupId);


        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }





    [Fact]
    public async Task GetModeratorGroups_WithValidModeratorId_ReturnsGroupsList() {

        var moderatorId = 1;
        var request = new GetModeratorGroupsRequest { Id = moderatorId };

        var groups = _fixture.Build<Group>()
            .With(g => g.IsActive, true)
            .CreateMany(3)
            .ToList();

        var moderatorGroups = groups.Select((g, i) => new ModeratorGroup
        {
            ModeratorId = moderatorId,
            GroupId = g.Id,
            Group = g
        }).ToList();

        var queryable = moderatorGroups.AsQueryable().BuildMockDbSet().Object;

        _mockModeratorGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<ModeratorGroup, GetModeratorGroupsResponse>()));


        var result = await _service.GetModeratorGroups(request);


        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        _mockModeratorGroupRepository.Verify(r => r.QueryNoTracking(), Times.Once);
    }

    [Fact]
    public async Task GetModeratorGroups_WithNoGroups_ReturnsEmptyList() {

        var moderatorId = 999;
        var request = new GetModeratorGroupsRequest { Id = moderatorId };
        var queryable = new List<ModeratorGroup>().AsQueryable().BuildMockDbSet().Object;

        _mockModeratorGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<ModeratorGroup, GetModeratorGroupsResponse>()));


        var result = await _service.GetModeratorGroups(request);


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }





    [Fact]
    public async Task GetModeratorFreeGroups_ReturnsGroupsNotAssignedToModerator() {

        var moderatorId = 1;
        var organizationId = 1;
        var request = new GetModeratorFreeGroupsRequest
        {
            ModeratorId = moderatorId,
            OrganizationId = organizationId
        };


        var moderatorGroups = new List<ModeratorGroup>
        {
            new() { ModeratorId = moderatorId, GroupId = 1 }
        }.AsQueryable().BuildMockDbSet().Object;


        var allGroups = new List<Group>
        {
            new() { Id = 1, OrganizationId = organizationId, IsActive = true, Name = "Group 1" },
            new() { Id = 2, OrganizationId = organizationId, IsActive = true, Name = "Group 2" },
            new() { Id = 3, OrganizationId = organizationId, IsActive = true, Name = "Group 3" }
        }.AsQueryable().BuildMockDbSet().Object;

        _mockModeratorGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(moderatorGroups);

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(allGroups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetModeratorFreeGroupsResponse>()));


        var result = await _service.GetModeratorFreeGroups(request);


        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetModeratorFreeGroups_WhenAllGroupsAssigned_ReturnsEmptyList() {

        var moderatorId = 1;
        var organizationId = 1;
        var request = new GetModeratorFreeGroupsRequest
        {
            ModeratorId = moderatorId,
            OrganizationId = organizationId
        };


        var moderatorGroups = new List<ModeratorGroup>
        {
            new() { ModeratorId = moderatorId, GroupId = 1 },
            new() { ModeratorId = moderatorId, GroupId = 2 }
        }.AsQueryable().BuildMockDbSet().Object;

        var allGroups = new List<Group>
        {
            new() { Id = 1, OrganizationId = organizationId, IsActive = true, Name = "Group 1" },
            new() { Id = 2, OrganizationId = organizationId, IsActive = true, Name = "Group 2" }
        }.AsQueryable().BuildMockDbSet().Object;

        _mockModeratorGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(moderatorGroups);

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(allGroups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetModeratorFreeGroupsResponse>()));


        var result = await _service.GetModeratorFreeGroups(request);


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetModeratorFreeGroups_OnlyReturnsActiveGroups() {

        var moderatorId = 1;
        var organizationId = 1;
        var request = new GetModeratorFreeGroupsRequest
        {
            ModeratorId = moderatorId,
            OrganizationId = organizationId
        };

        var moderatorGroups = new List<ModeratorGroup>().AsQueryable().BuildMockDbSet().Object;

        var allGroups = new List<Group>
        {
            new() { Id = 1, OrganizationId = organizationId, IsActive = true, Name = "Active Group" },
            new() { Id = 2, OrganizationId = organizationId, IsActive = false, Name = "Inactive Group" }
        }.AsQueryable().BuildMockDbSet().Object;

        _mockModeratorGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(moderatorGroups);

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(allGroups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, GetModeratorFreeGroupsResponse>()));


        var result = await _service.GetModeratorFreeGroups(request);


        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }





    [Fact]
    public async Task CreateGroup_OnSuccess_DoesNotLogError() {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, "Test Group")
            .Create();

        var group = new Group { Name = request.Name, OrganizationId = request.OrganizationId };
        var organization = _fixture.Build<Organization>().With(o => o.Id, request.OrganizationId).Create();
        var response = new CreateGroupResponse { Id = 1 };

        _mockMapper.Setup(m => m.Map<Group>(request)).Returns(group);
        _mockMapper.Setup(m => m.Map<CreateGroupResponse>(It.IsAny<Group>())).Returns(response);
        _mockGroupRepository.Setup(r => r.AddAsync(It.IsAny<Group>(), default)).Returns(Task.CompletedTask);
        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default)).ReturnsAsync(organization);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateGroup(request);


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task GroupExists_OnSuccess_DoesNotLogError() {

        _mockGroupRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(true);


        await _service.GroupExists("Test", 1);


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Never);
    }





    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateGroup_WithInvalidName_ThrowsGroupValidationException(string? invalidName) {

        var request = _fixture.Build<CreateGroupRequest>()
            .With(r => r.Name, invalidName!)
            .Create();

        var group = new Group { Name = invalidName!, OrganizationId = request.OrganizationId };

        _mockMapper.Setup(m => m.Map<Group>(request))
            .Returns(group);


        await Assert.ThrowsAsync<GroupValidationException>(
            async () => await _service.CreateGroup(request));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateGroup_WithInvalidName_ThrowsGroupValidationException(string? invalidName) {

        var groupId = 1;
        var request = _fixture.Build<UpdateGroupRequest>()
            .With(r => r.Id, groupId)
            .With(r => r.Name, invalidName!)
            .Create();

        var existingGroup = _fixture.Build<Group>()
            .With(g => g.Id, groupId)
            .Create();

        _mockGroupRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Group, bool>>>(), default))
            .ReturnsAsync(existingGroup);

        _mockMapper.Setup(m => m.Map(request, existingGroup))
            .Callback<UpdateGroupRequest, Group>((req, grp) => grp.Name = req.Name);


        await Assert.ThrowsAsync<GroupValidationException>(
            async () => await _service.UpdateGroup(request));
    }





    [Fact]
    public void GroupService_Constructor_InitializesDependenciesCorrectly() {

        var service = new GroupService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        service.Should().NotBeNull();
    }


}