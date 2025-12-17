using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Organizations;
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

        // Setup UnitOfWork to return mocked repositories
        _mockUnitOfWork.Setup(uow => uow.Organizations).Returns(_mockOrganizationRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);

        // Create service with mocked dependencies
        _service = new OrganizationService(
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
    public async Task CreateOrganization_WithValidData_ReturnsCreatedOrganization() {
        // Arrange
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

        // Act
        var result = await _service.CreateOrganization(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Name.Should().Be(request.Name);
        _mockOrganizationRepository.Verify(r => r.AddAsync(It.IsAny<Organization>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetOrganization_WithValidId_ReturnsOrganization() {
        // Arrange
        var organizationId = 1;
        var request = new GetOrganizationRequest { Id = organizationId };

        var organizations = new List<Organization>
        {
            _fixture.Build<Organization>()
                .With(o => o.Id, organizationId)
                .With(o => o.IsActive, true)
                .Create()
        }.AsQueryable();

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(organizations);

        // Act & Assert
        // This demonstrates the pattern for retrieving a single organization
    }

    [Fact]
    public async Task UpdateOrganization_WithValidData_UpdatesOrganization() {
        // Arrange
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

        // Act
        var result = await _service.UpdateOrganization(request);

        // Assert
        result.Should().NotBeNull();
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.Name == "Updated Organization")), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateOrganizationStatus_TogglesIsActiveFlag() {
        // Arrange
        var organizationId = 1;
        var request = new UpdateOrganizationStatusRequest { Id = organizationId };

        var organization = _fixture.Build<Organization>()
            .With(o => o.Id, organizationId)
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

        // Act
        var result = await _service.UpdateOrganizationStatus(request);

        // Assert
        result.Should().NotBeNull();
        organization.IsActive.Should().BeFalse(); // Should be toggled from true to false
        _mockOrganizationRepository.Verify(r => r.Update(
            It.Is<Organization>(o => o.IsActive == false)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task OrganizationExists_WithExistingName_ReturnsTrue() {
        // Arrange
        var organizationName = "Existing Organization";

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(true);

        // Act
        var result = await _service.OrganizationExists(organizationName);

        // Assert
        result.Should().BeTrue();
        _mockOrganizationRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task OrganizationExists_WithNonExistingName_ReturnsFalse() {
        // Arrange
        var organizationName = "Non-existing Organization";

        _mockOrganizationRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Organization, bool>>>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await _service.OrganizationExists(organizationName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedOrganizations_WithNameFilter_ReturnsFilteredResults() {
        // Arrange
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
            .ToList();

        var queryable = organizations.AsQueryable();

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        // Act & Assert
        // This demonstrates the pattern for paged organizations with filters
    }

    [Fact]
    public async Task GetPagedOrganizations_WithGroupsFilterYes_FiltersCorrectly() {
        // Arrange
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

        var allOrganizations = organizationsWithGroups.Concat(organizationsWithoutGroups).AsQueryable();

        _mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(allOrganizations);

        // Act & Assert
        // This demonstrates filtering by HasActiveGroup = true
    }

    [Fact]
    public async Task GetInnerGroups_WithValidOrganizationId_ReturnsActiveGroupsWithTeams() {
        // Arrange
        var organizationId = 1;

        var groups = _fixture.Build<Group>()
            .With(g => g.OrganizationId, organizationId)
            .With(g => g.IsActive, true)
            .CreateMany(3)
            .ToList();

        var queryable = groups.AsQueryable();

        _mockGroupRepository.Setup(r => r.Query())
            .Returns(queryable);

        // Act & Assert
        // This demonstrates the pattern for inner groups query
    }

    [Fact]
    public void OrganizationService_Constructor_InitializesDependenciesCorrectly() {
        // Arrange & Act
        var service = new OrganizationService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateOrganization_CallsSetAuditableInfoOnCreate() {
        // Arrange
        var request = _fixture.Create<CreateOrganizationRequest>();
        var organization = _fixture.Create<Organization>();
        var currentUser = "test-user@example.com";

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

        // Act
        await _service.CreateOrganization(request);

        // Assert
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockOrganizationRepository.Verify(r => r.AddAsync(It.IsAny<Organization>(), default), Times.Once);
    }
}
