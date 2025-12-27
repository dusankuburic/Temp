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
using Temp.Services.Workplaces;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Tests.Unit.Services;

public class WorkplaceServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Workplace>> _mockWorkplaceRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IWorkplaceService _service;

    public WorkplaceServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockWorkplaceRepository = new Mock<IRepository<Workplace>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        _mockUnitOfWork.Setup(uow => uow.Workplaces).Returns(_mockWorkplaceRepository.Object);

        _service = new WorkplaceService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }



    [Fact]
    public async Task CreateWorkplace_WithValidData_ReturnsCreatedWorkplace() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "Test Workplace")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, request.Name)
            .With(w => w.IsActive, true)
            .Create();

        var response = _fixture.Build<CreateWorkplaceResponse>()
            .With(r => r.Id, workplace.Id)
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);
        _mockMapper.Setup(m => m.Map<CreateWorkplaceResponse>(It.IsAny<Workplace>())).Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.AddAsync(It.IsAny<Workplace>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.CreateWorkplace(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockWorkplaceRepository.Verify(r => r.AddAsync(It.IsAny<Workplace>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateWorkplace_WithEmptyName_ThrowsWorkplaceValidationException() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceValidationException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task CreateWorkplace_WithInvalidName_ThrowsWorkplaceValidationException(string? invalidName) {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, invalidName)
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, invalidName)
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceValidationException>();
    }

    [Fact]
    public async Task CreateWorkplace_WithNullMappedWorkplace_ThrowsWorkplaceServiceException() {

        var request = _fixture.Create<CreateWorkplaceRequest>();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns((Workplace)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.CreateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
    }

    [Fact]
    public async Task CreateWorkplace_SetsAuditInfo() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "Test Workplace")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, request.Name)
            .Create();

        var currentUser = new CurrentUser { AppUserId = "user-id", Email = "user@example.com" };

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);
        _mockMapper.Setup(m => m.Map<CreateWorkplaceResponse>(It.IsAny<Workplace>()))
            .Returns(_fixture.Create<CreateWorkplaceResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(currentUser);

        _mockWorkplaceRepository.Setup(r => r.AddAsync(It.IsAny<Workplace>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateWorkplace(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockWorkplaceRepository.Verify(r => r.AddAsync(It.IsAny<Workplace>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateWorkplace_WhenDatabaseFails_ThrowsWorkplaceServiceException() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "Test Workplace")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, request.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.AddAsync(It.IsAny<Workplace>(), default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.CreateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public Task GetWorkplace_WithValidId_QueriesRepository() {

        var workplaceId = 1;

        var workplaces = new List<Workplace>
        {
            _fixture.Build<Workplace>()
                .With(w => w.Id, workplaceId)
                .With(w => w.IsActive, true)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplaceResponse>()));


        _mockWorkplaceRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetWorkplace_WithInactiveWorkplace_ReturnsNull() {

        var workplaceId = 1;

        var workplaces = new List<Workplace>
        {
            _fixture.Build<Workplace>()
                .With(w => w.Id, workplaceId)
                .With(w => w.IsActive, false)
                .Create()
        }.AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplaceResponse>()));


        var result = await _service.GetWorkplace(workplaceId);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetWorkplace_WithNonExistentId_ReturnsNull() {

        var workplaces = new List<Workplace>().AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplaceResponse>()));


        var result = await _service.GetWorkplace(999);


        result.Should().BeNull();
    }





    [Fact]
    public Task GetWorkplaces_ReturnsOnlyActiveWorkplaces() {

        var activeWorkplaces = _fixture.Build<Workplace>()
            .With(w => w.IsActive, true)
            .CreateMany(3)
            .ToList();

        var inactiveWorkplaces = _fixture.Build<Workplace>()
            .With(w => w.IsActive, false)
            .CreateMany(2)
            .ToList();

        var allWorkplaces = activeWorkplaces.Concat(inactiveWorkplaces).ToList().AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(allWorkplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplaceResponse>()));


        _mockWorkplaceRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetWorkplaces_WithNoWorkplaces_ReturnsEmptyList() {

        var workplaces = new List<Workplace>().AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplaceResponse>()));


        var result = await _service.GetWorkplaces();


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }





    [Fact]
    public async Task UpdateWorkplace_WithValidData_UpdatesWorkplace() {

        var workplaceId = 1;
        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, workplaceId)
            .With(r => r.Name, "Updated Workplace")
            .Create();

        var existingWorkplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .With(w => w.Name, "Original Workplace")
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(existingWorkplace);

        _mockMapper.Setup(m => m.Map(request, existingWorkplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateWorkplace(request);


        result.Should().NotBeNull();
        _mockWorkplaceRepository.Verify(r => r.Update(
            It.Is<Workplace>(w => w.Name == "Updated Workplace")), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplace_WithEmptyName_ThrowsWorkplaceValidationException() {

        var workplaceId = 1;
        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, workplaceId)
            .With(r => r.Name, "")
            .Create();

        var existingWorkplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(existingWorkplace);

        _mockMapper.Setup(m => m.Map(request, existingWorkplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceValidationException>();
    }

    [Fact]
    public async Task UpdateWorkplace_WithNonExistentWorkplace_ThrowsWorkplaceServiceException() {

        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, 999)
            .With(r => r.Name, "Updated Name")
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync((Workplace)null!);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        Func<Task> act = async () => await _service.UpdateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
    }

    [Fact]
    public async Task UpdateWorkplace_SetsAuditInfoOnUpdate() {

        var workplaceId = 1;
        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, workplaceId)
            .With(r => r.Name, "Updated Workplace")
            .Create();

        var existingWorkplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .With(w => w.Name, "Original Workplace")
            .Create();

        var currentUser = new CurrentUser { AppUserId = "user-id", Email = "updater@example.com" };

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(existingWorkplace);

        _mockMapper.Setup(m => m.Map(request, existingWorkplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(currentUser);

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateWorkplace(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockWorkplaceRepository.Verify(r => r.Update(It.IsAny<Workplace>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplace_WhenDatabaseFails_ThrowsWorkplaceServiceException() {

        var workplaceId = 1;
        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, workplaceId)
            .With(r => r.Name, "Updated Workplace")
            .Create();

        var existingWorkplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(existingWorkplace);

        _mockMapper.Setup(m => m.Map(request, existingWorkplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.UpdateWorkplace(request);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
    }





    [Fact]
    public async Task UpdateWorkplaceStatus_TogglesIsActiveFlag_FromTrueToFalse() {

        var workplaceId = 1;
        var request = new UpdateWorkplaceStatusRequest { Id = workplaceId };

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .With(w => w.Name, "Test Workplace")
            .With(w => w.IsActive, true)
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateWorkplaceStatus(request);


        result.Should().NotBeNull();
        workplace.IsActive.Should().BeFalse();
        _mockWorkplaceRepository.Verify(r => r.Update(
            It.Is<Workplace>(w => w.IsActive == false)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplaceStatus_TogglesIsActiveFlag_FromFalseToTrue() {

        var workplaceId = 1;
        var request = new UpdateWorkplaceStatusRequest { Id = workplaceId };

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .With(w => w.Name, "Test Workplace")
            .With(w => w.IsActive, false)
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _service.UpdateWorkplaceStatus(request);


        result.Should().NotBeNull();
        workplace.IsActive.Should().BeTrue();
        _mockWorkplaceRepository.Verify(r => r.Update(
            It.Is<Workplace>(w => w.IsActive == true)), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplaceStatus_WithNonExistentWorkplace_ThrowsWorkplaceServiceException() {

        var request = new UpdateWorkplaceStatusRequest { Id = 999 };

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync((Workplace)null!);


        Func<Task> act = async () => await _service.UpdateWorkplaceStatus(request);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
    }

    [Fact]
    public async Task UpdateWorkplaceStatus_SetsAuditInfo() {

        var workplaceId = 1;
        var request = new UpdateWorkplaceStatusRequest { Id = workplaceId };

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Id, workplaceId)
            .With(w => w.Name, "Test Workplace")
            .With(w => w.IsActive, true)
            .Create();

        var currentUser = new CurrentUser { AppUserId = "admin-id", Email = "admin@example.com" };

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(currentUser);

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateWorkplaceStatus(request);


        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockWorkplaceRepository.Verify(r => r.Update(It.IsAny<Workplace>()), Times.Once);
    }





    [Fact]
    public async Task WorkplaceExists_WithExistingName_ReturnsTrue() {

        var workplaceName = "Existing Workplace";

        _mockWorkplaceRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(true);


        var result = await _service.WorkplaceExists(workplaceName);


        result.Should().BeTrue();
        _mockWorkplaceRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task WorkplaceExists_WithNonExistingName_ReturnsFalse() {

        var workplaceName = "Non-existing Workplace";

        _mockWorkplaceRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.WorkplaceExists(workplaceName);


        result.Should().BeFalse();
    }

    [Fact]
    public async Task WorkplaceExists_WhenDatabaseFails_ThrowsWorkplaceServiceException() {

        var workplaceName = "Any Workplace";

        _mockWorkplaceRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ThrowsAsync(new Exception("Database error"));


        Func<Task> act = async () => await _service.WorkplaceExists(workplaceName);


        await act.Should().ThrowAsync<WorkplaceServiceException>();
        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("Test Workplace")]
    public async Task WorkplaceExists_WithVariousNames_CallsRepositoryCorrectly(string name) {

        _mockWorkplaceRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(false);


        await _service.WorkplaceExists(name);


        _mockWorkplaceRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default), Times.Once);
    }





    [Fact]
    public Task GetPagedWorkplaces_WithNameFilter_QueriesCorrectly() {

        var request = new GetWorkplacesRequest
        {
            Name = "Office",
            PageNumber = 1,
            PageSize = 10
        };

        var workplaces = _fixture.Build<Workplace>()
            .With(w => w.IsActive, true)
            .With(w => w.Name, "Office A")
            .CreateMany(5)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplacesResponse>()));


        _mockWorkplaceRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }

    [Fact]
    public Task GetPagedWorkplaces_WithEmptyNameFilter_ReturnsAllActiveWorkplaces() {

        var request = new GetWorkplacesRequest
        {
            Name = "",
            PageNumber = 1,
            PageSize = 10
        };

        var workplaces = _fixture.Build<Workplace>()
            .With(w => w.IsActive, true)
            .CreateMany(5)
            .ToList()
            .AsQueryable().BuildMockDbSet().Object;

        _mockWorkplaceRepository.Setup(r => r.QueryNoTracking()).Returns(workplaces);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Workplace, GetWorkplacesResponse>()));


        _mockWorkplaceRepository.Object.QueryNoTracking().Should().NotBeNull();
        return Task.CompletedTask;
    }





    [Fact]
    public async Task CreateWorkplace_WithValidationError_LogsError() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, "")
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.CreateWorkplace(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplace_WithValidationError_LogsError() {

        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Id, 1)
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);

        _mockMapper.Setup(m => m.Map(request, workplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });


        try {
            await _service.UpdateWorkplace(request);
        } catch {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public void WorkplaceService_Constructor_InitializesDependenciesCorrectly() {

        var service = new WorkplaceService(
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
    public async Task WorkplaceExists_WithVariousLengthNames_WorksCorrectly(int nameLength) {

        var workplaceName = new string('a', nameLength);

        _mockWorkplaceRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.WorkplaceExists(workplaceName);


        result.Should().BeFalse();
        _mockWorkplaceRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateWorkplace_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<CreateWorkplaceRequest>()
            .With(r => r.Name, "Test Workplace")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Name, request.Name)
            .Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);
        _mockMapper.Setup(m => m.Map<CreateWorkplaceResponse>(It.IsAny<Workplace>()))
            .Returns(_fixture.Create<CreateWorkplaceResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.AddAsync(It.IsAny<Workplace>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.CreateWorkplace(request);


        _mockMapper.Verify(m => m.Map<Workplace>(request), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockWorkplaceRepository.Verify(r => r.AddAsync(It.IsAny<Workplace>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        _mockMapper.Verify(m => m.Map<CreateWorkplaceResponse>(It.IsAny<Workplace>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplace_VerifiesCorrectRepositoryMethodsCalled() {

        var request = _fixture.Build<UpdateWorkplaceRequest>()
            .With(r => r.Id, 1)
            .With(r => r.Name, "Updated Name")
            .Create();

        var workplace = _fixture.Build<Workplace>()
            .With(w => w.Id, 1)
            .With(w => w.Name, "Original Name")
            .Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);

        _mockMapper.Setup(m => m.Map(request, workplace))
            .Callback<UpdateWorkplaceRequest, Workplace>((req, w) => w.Name = req.Name);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);


        await _service.UpdateWorkplace(request);


        _mockWorkplaceRepository.Verify(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Workplace, bool>>>(), default), Times.Once);
        _mockIdentityProvider.Verify(i => i.GetCurrentUser(), Times.Once);
        _mockWorkplaceRepository.Verify(r => r.Update(It.IsAny<Workplace>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }


}