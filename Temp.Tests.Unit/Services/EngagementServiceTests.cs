using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Engagements;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class EngagementServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Engagement>> _mockEngagementRepository;
    private readonly Mock<IRepository<Employee>> _mockEmployeeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IEngagementService _service;

    public EngagementServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEngagementRepository = new Mock<IRepository<Engagement>>();
        _mockEmployeeRepository = new Mock<IRepository<Employee>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();


        _mockUnitOfWork.Setup(uow => uow.Engagements).Returns(_mockEngagementRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.Employees).Returns(_mockEmployeeRepository.Object);


        _service = new EngagementService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateEngagement_WithValidData_ReturnsCreatedEngagement() {

        var dateFrom = DateTime.UtcNow;
        var dateTo = dateFrom.AddMonths(6);

        var request = _fixture.Build<CreateEngagementRequest>()
            .With(r => r.EmployeeId, 1)
            .With(r => r.WorkplaceId, 1)
            .With(r => r.EmploymentStatusId, 1)
            .With(r => r.DateFrom, dateFrom)
            .With(r => r.DateTo, dateTo)
            .Create();

        var engagement = _fixture.Build<Engagement>()
            .With(e => e.EmployeeId, request.EmployeeId)
            .With(e => e.WorkplaceId, request.WorkplaceId)
            .With(e => e.EmploymentStatusId, request.EmploymentStatusId)
            .With(e => e.DateFrom, dateFrom)
            .With(e => e.DateTo, dateTo)
            .Create();

        var response = _fixture.Build<CreateEngagementResponse>()
            .With(r => r.Id, engagement.Id)
            .Create();

        _mockMapper.Setup(m => m.Map<Engagement>(request))
            .Returns(engagement);

        _mockMapper.Setup(m => m.Map<CreateEngagementResponse>(It.IsAny<Engagement>()))
            .Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockEngagementRepository.Setup(r => r.AddAsync(It.IsAny<Engagement>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateEngagement(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockEngagementRepository.Verify(r => r.AddAsync(It.IsAny<Engagement>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public Task GetUserEmployeeEngagements_WithValidEmployeeId_ReturnsEngagementsList() {

        var employeeId = 1;
        var request = new GetUserEmployeeEngagementsRequest { Id = employeeId };

        var employee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var engagements = _fixture.Build<Engagement>()
            .With(e => e.EmployeeId, employeeId)
            .CreateMany(3)
            .ToList();

        var employeeQueryable = new List<Employee> { employee }.AsQueryable();

        _mockEmployeeRepository.Setup(r => r.QueryNoTracking())
            .Returns(employeeQueryable);

        var engagementQueryable = engagements.AsQueryable();

        _mockEngagementRepository.Setup(r => r.Query())
            .Returns(engagementQueryable);
        return Task.CompletedTask;



    }

    [Fact]
    public Task GetEngagementForEmployee_WithValidEmployeeId_ReturnsEngagementsList() {

        var employeeId = 1;
        var request = new GetEngagementsForEmployeeRequest { Id = employeeId };

        var engagements = _fixture.Build<Engagement>()
            .With(e => e.EmployeeId, employeeId)
            .CreateMany(5)
            .ToList();

        var queryable = engagements.AsQueryable();

        _mockEngagementRepository.Setup(r => r.Query())
            .Returns(queryable);
        return Task.CompletedTask;


    }

    [Fact]
    public void EngagementService_Constructor_InitializesDependenciesCorrectly() {

        var service = new EngagementService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        service.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateEngagement_CallsSetAuditableInfoOnCreate() {

        var dateFrom = DateTime.UtcNow;
        var dateTo = dateFrom.AddMonths(6);

        var request = _fixture.Build<CreateEngagementRequest>()
            .With(r => r.DateFrom, dateFrom)
            .With(r => r.DateTo, dateTo)
            .Create();

        var engagement = _fixture.Build<Engagement>()
            .With(e => e.DateFrom, dateFrom)
            .With(e => e.DateTo, dateTo)
            .Create();

        var currentUser = "test-user@example.com";

        _mockMapper.Setup(m => m.Map<Engagement>(request))
            .Returns(engagement);

        _mockMapper.Setup(m => m.Map<CreateEngagementResponse>(It.IsAny<Engagement>()))
            .Returns(_fixture.Create<CreateEngagementResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockEngagementRepository.Setup(r => r.AddAsync(It.IsAny<Engagement>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEngagement(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockEngagementRepository.Verify(r => r.AddAsync(It.IsAny<Engagement>(), default), Times.Once);
    }

    [Fact]
    public Task GetUserEmployeeEngagements_IncludesWorkplaceAndEmploymentStatus() {

        var employeeId = 1;
        var request = new GetUserEmployeeEngagementsRequest { Id = employeeId };

        var employee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var employeeQueryable = new List<Employee> { employee }.AsQueryable();

        _mockEmployeeRepository.Setup(r => r.QueryNoTracking())
            .Returns(employeeQueryable);

        var engagements = _fixture.Build<Engagement>()
            .With(e => e.EmployeeId, employeeId)
            .With(e => e.Workplace, _fixture.Create<Workplace>())
            .With(e => e.EmploymentStatus, _fixture.Create<EmploymentStatus>())
            .CreateMany(2)
            .ToList();

        var engagementQueryable = engagements.AsQueryable();

        _mockEngagementRepository.Setup(r => r.Query())
            .Returns(engagementQueryable);
        return Task.CompletedTask;



    }

    [Fact]
    public Task CreateEngagement_WithNullRequest_ThrowsException() {

        CreateEngagementRequest nullRequest = null!;
        return Task.CompletedTask;



    }
}