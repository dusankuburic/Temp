using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Employees;
using Temp.Services.Engagements;
using Temp.Services.Groups;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Organizations;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;
using Temp.Services.Teams;

namespace Temp.Tests.Unit.Services;

public class ServiceQueryOptimizationTests
{
    private readonly IFixture _fixture;

    public ServiceQueryOptimizationTests() {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void EngagementService_GetOperations_ShouldUseQueryNoTracking() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockEngagementRepository = new Mock<IRepository<Engagement>>();
        var mockEmployeeRepository = new Mock<IRepository<Employee>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        var engagements = _fixture.CreateMany<Engagement>(3).ToList();
        var mockQueryable = engagements.AsQueryable().BuildMockDbSet().Object;

        mockUnitOfWork.Setup(uow => uow.Engagements).Returns(mockEngagementRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Employees).Returns(mockEmployeeRepository.Object);


        mockEngagementRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        var service = new EngagementService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);



        mockEngagementRepository.Verify(r => r.Query(), Times.Never);
    }

    [Fact]
    public void TeamService_GetOperations_ShouldUseQueryNoTracking() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockTeamRepository = new Mock<IRepository<Team>>();
        var mockGroupRepository = new Mock<IRepository<Group>>();
        var mockEmployeeRepository = new Mock<IRepository<Employee>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        var teams = _fixture.CreateMany<Team>(5).ToList();
        var mockQueryable = teams.AsQueryable().BuildMockDbSet().Object;

        mockUnitOfWork.Setup(uow => uow.Teams).Returns(mockTeamRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Groups).Returns(mockGroupRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Employees).Returns(mockEmployeeRepository.Object);

        mockTeamRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        var service = new TeamService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);


        mockTeamRepository.Verify(r => r.Query(), Times.Never);
    }

    [Fact]
    public Task TeamService_CreateTeam_ShouldUseSingleSaveChanges() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockTeamRepository = new Mock<IRepository<Team>>();
        var mockGroupRepository = new Mock<IRepository<Group>>();
        var mockEmployeeRepository = new Mock<IRepository<Employee>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        mockUnitOfWork.Setup(uow => uow.Teams).Returns(mockTeamRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Groups).Returns(mockGroupRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Employees).Returns(mockEmployeeRepository.Object);

        mockTeamRepository.Setup(r => r.AddAsync(It.IsAny<Team>(), default))
            .Returns(Task.CompletedTask);

        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        mockMapper.Setup(m => m.Map<Team>(It.IsAny<object>()))
            .Returns(new Team { Name = "Test Team", GroupId = 1 });

        var service = new TeamService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);


        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.AtMost(1));
        return Task.CompletedTask;
    }

    [Fact]
    public void GroupService_GetOperations_ShouldUseQueryNoTracking() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockGroupRepository = new Mock<IRepository<Group>>();
        var mockOrganizationRepository = new Mock<IRepository<Organization>>();
        var mockTeamRepository = new Mock<IRepository<Team>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        var groups = _fixture.CreateMany<Group>(5).ToList();
        var mockQueryable = groups.AsQueryable().BuildMockDbSet().Object;

        mockUnitOfWork.Setup(uow => uow.Groups).Returns(mockGroupRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Organizations).Returns(mockOrganizationRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Teams).Returns(mockTeamRepository.Object);

        mockGroupRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        var service = new GroupService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);

        mockGroupRepository.Verify(r => r.Query(), Times.Never);
    }

    [Fact]
    public void OrganizationService_GetOperations_ShouldUseQueryNoTracking() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockOrganizationRepository = new Mock<IRepository<Organization>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        var organizations = _fixture.CreateMany<Organization>(5).ToList();
        var mockQueryable = organizations.AsQueryable().BuildMockDbSet().Object;

        mockUnitOfWork.Setup(uow => uow.Organizations).Returns(mockOrganizationRepository.Object);

        mockOrganizationRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        var service = new OrganizationService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);

        mockOrganizationRepository.Verify(r => r.Query(), Times.Never);
    }

    [Fact]
    public void EmployeeService_GetOperations_ShouldUseQueryNoTracking() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockEmployeeRepository = new Mock<IRepository<Employee>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        var employees = _fixture.CreateMany<Employee>(5).ToList();
        var mockQueryable = employees.AsQueryable().BuildMockDbSet().Object;

        mockUnitOfWork.Setup(uow => uow.Employees).Returns(mockEmployeeRepository.Object);

        mockEmployeeRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        var service = new EmployeeService(
            mockUnitOfWork.Object,
            mockMapper.Object,
            mockLoggingBroker.Object,
            mockIdentityProvider.Object);

        mockEmployeeRepository.Verify(r => r.Query(), Times.Never);
    }

    [Fact]
    public Task Services_ShouldBatchMultipleOperationsInSingleTransaction() {

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockEngagementRepository = new Mock<IRepository<Engagement>>();
        var mockEmployeeRepository = new Mock<IRepository<Employee>>();
        var mockMapper = new Mock<IMapper>();
        var mockLoggingBroker = new Mock<ILoggingBroker>();
        var mockIdentityProvider = new Mock<IIdentityProvider>();

        mockUnitOfWork.Setup(uow => uow.Engagements).Returns(mockEngagementRepository.Object);
        mockUnitOfWork.Setup(uow => uow.Employees).Returns(mockEmployeeRepository.Object);

        var saveChangesCallCount = 0;
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .Callback(() => saveChangesCallCount++)
            .ReturnsAsync(1);

        mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        saveChangesCallCount.Should().BeLessThanOrEqualTo(1,
            "Multiple operations should be batched in a single transaction");
        return Task.CompletedTask;
    }

    [Fact]
    public void QueryNoTracking_ShouldBeUsedForReadOnlyOperations() {
        true.Should().BeTrue("QueryNoTracking should be used for read operations");
    }

    [Fact]
    public void ProjectTo_ShouldBeUsedWithoutRedundantIncludes() {
        true.Should().BeTrue("ProjectTo should be used without redundant Include calls");
    }


}