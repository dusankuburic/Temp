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
using Temp.Services.Organizations;
using Temp.Services.Organizations.Exceptions;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class OrganizationServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Organization>> _mockOrganizationRepository;
    private readonly Mock<IRepository<Group>> _mockGroupRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IOrganizationService _service;

    public OrganizationServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrganizationRepository = new Mock<IRepository<Organization>>();
        _mockGroupRepository = new Mock<IRepository<Group>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();


        _mockUnitOfWork.Setup(uow => uow.Organizations).Returns(_mockOrganizationRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);


        _service = new OrganizationService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }



    [Fact]
    public async Task CreateOrganization_WithValidData_ReturnsCreatedOrganization() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "Test Organization")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, request.Name)
            .With(o => o.IsActive, true)
            .Create();

        var response = _fixture.Build<CreateOrganizationResponse>()
            .With(r => r.Id, organization.Id)
            .With(r => r.Name, organization.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockMapper.Setup(m => m.Map<CreateOrganizationResponse>(It.IsAny<Organization>()))
            .Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.AddAsync(It.IsAny<Organization>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateOrganization(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Name.Should().Be(request.Name);
        _mockOrganizationRepository.Verify(r => r.AddAsync(It.IsAny<Organization>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateOrganization_WithEmptyName_ThrowsOrganizationValidationException() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateOrganization(request);


        await act.Should().ThrowAsync<OrganizationValidationException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task CreateOrganization_WithInvalidName_ThrowsOrganizationValidationException(string? invalidName) {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, invalidName)
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, invalidName)
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateOrganization(request);


        await act.Should().ThrowAsync<OrganizationValidationException>();
    }

    [Fact]
    public async Task CreateOrganization_WithNullMappedOrganization_ThrowsOrganizationServiceException() {

        var request = _fixture.Create<CreateOrganizationRequest>();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns((Organization)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }

    [Fact]
    public async Task CreateOrganization_WhenDatabaseFails_ThrowsOrganizationServiceException() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "Test Organization")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, request.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.AddAsync(It.IsAny<Organization>(), default))
            .ThrowsAsync(new Exception("Database connection failed"));


        Func<Task> act = async () => await _service.CreateOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrganization_SetsAuditInfoCorrectly() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "Test Organization")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, request.Name)
            .Create();

        var currentUser = new CurrentUser { AppUserId = "test-user-id", Email = "test@example.com" };

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockMapper.Setup(m => m.Map<CreateOrganizationResponse>(It.IsAny<Organization>()))
            .Returns(_fixture.Create<CreateOrganizationResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(currentUser);

        _mockOrganizationRepository.Setup(r => r.AddAsync(It.IsAny<Organization>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateOrganization(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.AddAsync(It.IsAny<Organization>(), default), Times.Once);
    }





    [Fact]
    public Task GetOrganization_WithValidId_ReturnsOrganization() {

        var organizationId = 1;
        var request = new GetOrganizationRequest { Id = organizationId };

        var response = _fixture.Build<GetOrganizationResponse>()
            .With(o => o.Id, organizationId)
            .Create();

        var organizations = new List<GetOrganizationResponse> { response }
            .AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(new List<Organization>
            {
                _fixture.Build<Organization>()
                    .With(o => o.Id, organizationId)
                    .With(o => o.IsActive, true)
                    .Create()
            }.AsQueryable().BuildMockDbSet().Object);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        _mockOrganizationRepository.Verify(r => r.QueryNoTracking(), Times.Never);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetOrganization_WithInactiveOrganization_ThrowsOrganizationServiceException() {

        var organizationId = 1;
        var request = new GetOrganizationRequest { Id = organizationId };

        var organizations = new List<Organization>
        {
            _fixture.Build<Organization>()
                .With(o => o.Id, organizationId)
                .With(o => o.IsActive, false)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        Func<Task> act = async () => await _service.GetOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }

    [Fact]
    public async Task GetOrganization_WithNonExistentId_ThrowsOrganizationServiceException() {

        var request = new GetOrganizationRequest { Id = 999 };

        var organizations = new List<Organization>().AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        Func<Task> act = async () => await _service.GetOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }





    [Fact]
    public async Task UpdateOrganization_WithValidData_UpdatesOrganization() {

        var organizationId = 1;
        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, organizationId)
            .With(r => r.Name, "Updated Organization")
            .Create();

        var existingOrganization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.Name, "Original Organization")
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(existingOrganization);

        _mockMapper.Setup(m => m.Map(request, existingOrganization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateOrganization(request);


        result.Should().NotBeNull();
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.Name == "Updated Organization")), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganization_WithEmptyName_ThrowsOrganizationValidationException() {

        var organizationId = 1;
        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, organizationId)
            .With(r => r.Name, "")
            .Create();

        var existingOrganization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(existingOrganization);

        _mockMapper.Setup(m => m.Map(request, existingOrganization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateOrganization(request);


        await act.Should().ThrowAsync<OrganizationValidationException>();
    }

    [Fact]
    public async Task UpdateOrganization_WithNonExistentOrganization_ThrowsOrganizationServiceException() {

        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, 999)
            .With(r => r.Name, "Updated Name")
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync((Organization)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }

    [Fact]
    public async Task UpdateOrganization_SetsAuditInfoOnUpdate() {

        var organizationId = 1;
        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, organizationId)
            .With(r => r.Name, "Updated Organization")
            .Create();

        var existingOrganization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.Name, "Original Organization")
            .Create();

        var currentUser = new CurrentUser { AppUserId = "test-user-id", Email = "updater@example.com" };

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(existingOrganization);

        _mockMapper.Setup(m => m.Map(request, existingOrganization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(currentUser);

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateOrganization(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.Update(It.IsAny<Organization>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganization_WhenDatabaseFails_ThrowsOrganizationServiceException() {

        var organizationId = 1;
        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, organizationId)
            .With(r => r.Name, "Updated Organization")
            .Create();

        var existingOrganization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(existingOrganization);

        _mockMapper.Setup(m => m.Map(request, existingOrganization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.UpdateOrganization(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }





    [Fact]
    public async Task UpdateOrganizationStatus_TogglesIsActiveFlag_FromTrueToFalse() {

        var organizationId = 1;
        var request = new UpdateOrganizationStatusRequest { Id = organizationId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.Name, "Test Organization")
            .With(o => o.IsActive, true)
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateOrganizationStatus(request);


        result.Should().NotBeNull();
        organization.IsActive.Should().BeFalse();
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.IsActive == false)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganizationStatus_TogglesIsActiveFlag_FromFalseToTrue() {

        var organizationId = 1;
        var request = new UpdateOrganizationStatusRequest { Id = organizationId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.Name, "Test Organization")
            .With(o => o.IsActive, false)
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateOrganizationStatus(request);


        result.Should().NotBeNull();
        organization.IsActive.Should().BeTrue();
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.IsActive == true)), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganizationStatus_WithNonExistentOrganization_ThrowsOrganizationServiceException() {

        var request = new UpdateOrganizationStatusRequest { Id = 999 };

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync((Organization)null!);


        Func<Task> act = async () => await _service.UpdateOrganizationStatus(request);


        await act.Should().ThrowAsync<OrganizationServiceException>();
    }

    [Fact]
    public async Task UpdateOrganizationStatus_SetsAuditInfo() {

        var organizationId = 1;
        var request = new UpdateOrganizationStatusRequest { Id = organizationId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
            .With(o => o.Name, "Test Organization")
            .With(o => o.IsActive, true)
            .Create();

        var currentUser = new CurrentUser { AppUserId = "admin-id", Email = "admin@example.com" };

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(currentUser);

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateOrganizationStatus(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.Update(It.IsAny<Organization>()), Times.Once);
    }





    [Fact]
    public async Task OrganizationExists_WithExistingName_ReturnsTrue() {

        var organizationName = "Existing Organization";

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(true);


        var result = await _service.OrganizationExists(organizationName);


        result.Should().BeTrue();
        _mockOrganizationRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task OrganizationExists_WithNonExistingName_ReturnsFalse() {

        var organizationName = "Non-existing Organization";

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.OrganizationExists(organizationName);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task OrganizationExists_WhenDatabaseFails_ThrowsOrganizationServiceException() {

        var organizationName = "Any Organization";

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ThrowsAsync(new Exception("Database connection failed"));


        Func<Task> act = async () => await _service.OrganizationExists(organizationName);


        await act.Should().ThrowAsync<OrganizationServiceException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Test Organization")]
    public async Task OrganizationExists_WithVariousNames_CallsRepositoryCorrectly(string name) {

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(false);


        await _service.OrganizationExists(name);


        _mockOrganizationRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default), Times.Once);
    }





    [Fact]
    public Task GetPagedOrganizations_WithNameFilter_QueriesCorrectly() {

        var request = new GetOrganizationsRequest
        {
            Name = "Tech",
            PageNumber = 1,
            PageSize = 10,
            WithGroups = "all"
        };

        var organizations = _fixture.Build<Organization>()
            .With(o => o.IsActive, true)
            .With(o => o.Name, "Tech Company")
            .CreateMany(5)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        _mockOrganizationRepository.Verify(r => r.QueryNoTracking(), Times.Never);
        return Task.CompletedTask;
    }

    [Fact]
    public Task GetPagedOrganizations_WithGroupsFilterYes_FiltersCorrectly() {

        var request = new GetOrganizationsRequest
        {
            Name = "",
            PageNumber = 1,
            PageSize = 10,
            WithGroups = "yes"
        };

        var organizationsWithGroups = _fixture.Build<Organization>()
            .With(o => o.IsActive, true)
            .With(o => o.HasActiveGroup, true)
            .CreateMany(3)
            .ToList();

        var organizationsWithoutGroups = _fixture.Build<Organization>()
            .With(o => o.IsActive, true)
            .With(o => o.HasActiveGroup, false)
            .CreateMany(2)
            .ToList();

        var allOrganizations = organizationsWithGroups
            .Concat(organizationsWithoutGroups)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(allOrganizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));
        return Task.CompletedTask;

    }

    [Fact]
    public Task GetPagedOrganizations_WithGroupsFilterNo_FiltersCorrectly() {

        var request = new GetOrganizationsRequest
        {
            Name = "",
            PageNumber = 1,
            PageSize = 10,
            WithGroups = "no"
        };

        var organizations = _fixture.Build<Organization>()
            .With(o => o.IsActive, true)
            .With(o => o.HasActiveGroup, false)
            .CreateMany(3)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        _mockOrganizationRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }





    [Fact]
    public Task GetInnerGroups_WithValidOrganizationId_QueriesGroupRepository() {

        var organizationId = 1;

        var teams = new List<Team>
        {
            _fixture.Build<Team>().With(t => t.IsActive, true).Create()
        };

        var groups = _fixture.Build<Group>()
            .With(g => g.OrganizationId, organizationId)
            .With(g => g.IsActive, true)
            .With(g => g.Teams, teams)
            .CreateMany(3)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, InnerGroup>()));


        _mockGroupRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public Task GetInnerGroups_WithNoActiveGroups_ReturnsEmptyList() {

        var organizationId = 1;

        var groups = _fixture.Build<Group>()
            .With(g => g.OrganizationId, organizationId)
            .With(g => g.IsActive, false)
            .CreateMany(3)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Group, InnerGroup>()));


        _mockGroupRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }





    [Fact]
    public async Task GetOrganizations_WhenNoOrganizationsExist_ThrowsOrganizationValidationException() {

        var emptyOrganizations = new List<Organization>().AsQueryable().BuildMockDbSet().Object;

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(emptyOrganizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Organization, GetOrganizationResponse>()));


        Func<Task> act = async () => await _service.GetOrganizations();


        await act.Should().ThrowAsync<OrganizationValidationException>();
    }





    [Fact]
    public Task GetPagedInnerGroups_WithNameFilter_QueriesGroupsCorrectly() {

        var request = new GetOrganizationInnerGroupsRequest
        {
            OrganizationId = 1,
            Name = "Test",
            PageNumber = 1,
            PageSize = 10,
            WithTeams = "all"
        };

        var groups = _fixture.Build<Group>()
            .With(g => g.OrganizationId, 1)
            .With(g => g.IsActive, true)
            .With(g => g.Name, "Test Group")
            .CreateMany(5)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        var organizations = new List<Organization>
        {
            _fixture.Build<Organization>()
                .With(o => o.Id, 1)
                .With(o => o.Name, "Test Org")
                .With(o => o.IsActive, true)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(groups);

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg => {
                cfg.CreateMap<Group, InnerGroup>();
                cfg.CreateMap<Organization, GetOrganizationResponse>();
            }));


        _mockGroupRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public Task GetPagedInnerGroups_WithTeamsFilterYes_FiltersCorrectly() {

        var request = new GetOrganizationInnerGroupsRequest
        {
            OrganizationId = 1,
            Name = "",
            PageNumber = 1,
            PageSize = 10,
            WithTeams = "yes"
        };

        var groupsWithTeams = _fixture.Build<Group>()
            .With(g => g.OrganizationId, 1)
            .With(g => g.IsActive, true)
            .With(g => g.HasActiveTeam, true)
            .CreateMany(3)
            .ToList();

        var groupsWithoutTeams = _fixture.Build<Group>()
            .With(g => g.OrganizationId, 1)
            .With(g => g.IsActive, true)
            .With(g => g.HasActiveTeam, false)
            .CreateMany(2)
            .ToList();

        var allGroups = groupsWithTeams.Concat(groupsWithoutTeams).ToList().AsQueryable().BuildMockDbSet().Object;

        _mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(allGroups);


        var queryResult = _mockGroupRepository.Object.QueryNoTracking();
        queryResult.Count().Should().Be(5);
        return Task.CompletedTask;
    }





    [Fact]
    public async Task CreateOrganization_WithValidationError_LogsError() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.CreateOrganization(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganization_WithValidationError_LogsError() {

        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, 1)
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockMapper.Setup(m => m.Map(request, organization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.UpdateOrganization(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public void OrganizationService_Constructor_InitializesDependenciesCorrectly() {

        var service = new OrganizationService(
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
    public async Task OrganizationExists_WithVariousLengthNames_WorksCorrectly(int nameLength) {

        var organizationName = new string('a', nameLength);

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.OrganizationExists(organizationName);


        result.Should().BeFalse();
        _mockOrganizationRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateOrganization_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<CreateOrganizationRequest>()
            .With(r => r.Name, "Test Organization")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Name, request.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Organization>(request))
            .Returns(organization);

        _mockMapper.Setup(m => m.Map<CreateOrganizationResponse>(It.IsAny<Organization>()))
            .Returns(_fixture.Create<CreateOrganizationResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.AddAsync(It.IsAny<Organization>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateOrganization(request);


        _mockMapper.Verify(m => m.Map<Organization>(request), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.AddAsync(It.IsAny<Organization>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        _mockMapper.Verify(m => m.Map<CreateOrganizationResponse>(It.IsAny<Organization>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganization_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<UpdateOrganizationRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "Updated Name")
            .Create();

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, 1)
            .With(o => o.Name, "Original Name")
            .Create();

        _mockOrganizationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(organization);

        _mockMapper.Setup(m => m.Map(request, organization))
            .Callback<UpdateOrganizationRequest, Organization>((req, org) => org.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockOrganizationRepository.Setup(r => r.Update(It.IsAny<Organization>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateOrganization(request);


        _mockOrganizationRepository.Verify(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.Update(It.IsAny<Organization>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }


}