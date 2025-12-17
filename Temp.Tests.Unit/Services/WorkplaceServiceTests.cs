using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;
using Temp.Services.Workplaces;
using Temp.Services.Workplaces.Models.Commands;

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
        var request = _fixture.Build<CreateWorkplaceRequest>().With(r => r.Name, "Test Workplace").Create();
        var workplace = _fixture.Build<Workplace>().With(w => w.Name, request.Name).Create();
        var response = _fixture.Build<CreateWorkplaceResponse>().With(r => r.Id, workplace.Id).Create();

        _mockMapper.Setup(m => m.Map<Workplace>(request)).Returns(workplace);
        _mockMapper.Setup(m => m.Map<CreateWorkplaceResponse>(It.IsAny<Workplace>())).Returns(response);
        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });
        _mockWorkplaceRepository.Setup(r => r.AddAsync(It.IsAny<Workplace>(), default)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.CreateWorkplace(request);

        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockWorkplaceRepository.Verify(r => r.AddAsync(It.IsAny<Workplace>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplace_WithValidData_UpdatesWorkplace() {
        var workplaceId = 1;
        var request = _fixture.Build<UpdateWorkplaceRequest>().With(r => r.Id, workplaceId).Create();
        var existingWorkplace = _fixture.Build<Workplace>().With(w => w.Id, workplaceId).Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(existingWorkplace);
        _mockMapper.Setup(m => m.Map(request, existingWorkplace));
        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });
        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.UpdateWorkplace(request);

        result.Should().NotBeNull();
        _mockWorkplaceRepository.Verify(r => r.Update(It.IsAny<Workplace>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkplaceStatus_TogglesIsActiveFlag() {
        var workplaceId = 1;
        var request = new UpdateWorkplaceStatusRequest { Id = workplaceId };
        var workplace = _fixture.Build<Workplace>().With(w => w.Id, workplaceId).With(w => w.IsActive, true).Create();

        _mockWorkplaceRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Workplace, bool>>>(), default))
            .ReturnsAsync(workplace);
        _mockIdentityProvider.Setup(i => i.GetCurrentUser()).ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });
        _mockWorkplaceRepository.Setup(r => r.Update(It.IsAny<Workplace>()));
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.UpdateWorkplaceStatus(request);

        result.Should().NotBeNull();
        workplace.IsActive.Should().BeFalse();
        _mockWorkplaceRepository.Verify(r => r.Update(It.Is<Workplace>(w => w.IsActive == false)), Times.Once);
    }

    [Fact]
    public async Task WorkplaceExists_WithExistingName_ReturnsTrue() {
        var workplaceName = "Existing Workplace";
        _mockWorkplaceRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Workplace, bool>>>(), default)).ReturnsAsync(true);

        var result = await _service.WorkplaceExists(workplaceName);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task WorkplaceExists_WithNonExistingName_ReturnsFalse() {
        var workplaceName = "Non-existing Workplace";
        _mockWorkplaceRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Workplace, bool>>>(), default)).ReturnsAsync(false);

        var result = await _service.WorkplaceExists(workplaceName);

        result.Should().BeFalse();
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
}
