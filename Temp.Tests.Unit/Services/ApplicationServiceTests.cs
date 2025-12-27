using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models.Applications;
using Temp.Services.Applications;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class ApplicationServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Application>> _mockApplicationRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IApplicationService _service;

    public ApplicationServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockApplicationRepository = new Mock<IRepository<Application>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();


        _mockUnitOfWork.Setup(uow => uow.Applications).Returns(_mockApplicationRepository.Object);


        _service = new ApplicationService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateApplication_WithValidData_ReturnsCreatedApplication() {

        var request = _fixture.Build<CreateApplicationRequest>()
            .With(r => r.UserId, 1)
            .With(r => r.TeamId, 1)
            .Create();

        var application = _fixture.Build<Application>()
            .With(a => a.UserId, request.UserId)
            .With(a => a.TeamId, request.TeamId)
            .With(a => a.Status, false)
            .Create();

        var response = _fixture.Build<CreateApplicationResponse>()
            .With(r => r.Id, application.Id)
            .Create();

        _mockMapper.Setup(m => m.Map<Application>(request))
            .Returns(application);

        _mockMapper.Setup(m => m.Map<CreateApplicationResponse>(It.IsAny<Application>()))
            .Returns(response);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockApplicationRepository.Setup(r => r.AddAsync(It.IsAny<Application>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateApplication(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        _mockApplicationRepository.Verify(r => r.AddAsync(It.IsAny<Application>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public Task GetApplication_WithValidId_ReturnsApplication() {

        var applicationId = 1;
        var request = new GetApplicationRequest { Id = applicationId };

        var queryable = new List<Application>
        {
            _fixture.Build<Application>()
                .With(a => a.Id, applicationId)
                .Create()
        }.AsQueryable();

        var expectedResponse = _fixture.Build<GetApplicationResponse>()
            .With(r => r.Id, applicationId)
            .Create();

        _mockApplicationRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task UpdateApplicationStatus_WithValidData_UpdatesStatus() {

        var applicationId = 1;
        var request = _fixture.Build<UpdateApplicationStatusRequest>()
            .With(r => r.Id, applicationId)
            .Create();

        var existingApplication = _fixture.Build<Application>()
            .With(a => a.Id, applicationId)
            .With(a => a.Status, false)
            .Create();

        _mockApplicationRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Application, bool>>>(), default))
            .ReturnsAsync(existingApplication);

        _mockMapper.Setup(m => m.Map(request, existingApplication))
            .Callback<UpdateApplicationStatusRequest, Application>((req, app) => {

                app.ModeratorId = req.ModeratorId;
            });

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockApplicationRepository.Setup(r => r.Update(It.IsAny<Application>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateApplicationStatus(request);


        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(applicationId);
        _mockApplicationRepository.Verify(r => r.Update(It.IsAny<Application>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public Task GetUserApplications_WithValidUserId_ReturnsApplicationsList() {

        var userId = 1;
        var request = new GetUserApplicationsRequest { Id = userId };

        var applications = _fixture.Build<Application>()
            .With(a => a.UserId, userId)
            .CreateMany(3)
            .ToList();

        var queryable = applications.AsQueryable();

        _mockApplicationRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);
        return Task.CompletedTask;
    }

    [Fact]
    public Task GetTeamApplications_WithValidTeamId_ReturnsApplicationsList() {

        var teamId = 1;
        var request = new GetTeamApplicationsRequest { TeamId = teamId };

        var applications = _fixture.Build<Application>()
            .With(a => a.TeamId, teamId)
            .CreateMany(5)
            .ToList();

        var queryable = applications.AsQueryable();

        _mockApplicationRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);
        return Task.CompletedTask;
    }

    [Fact]
    public void ApplicationService_Constructor_InitializesDependenciesCorrectly() {

        var service = new ApplicationService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        service.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateApplication_SetsStatusToFalse() {

        var request = _fixture.Create<CreateApplicationRequest>();
        var application = _fixture.Build<Application>()
            .Without(a => a.Status)
            .Create();

        _mockMapper.Setup(m => m.Map<Application>(request))
            .Returns(application);

        _mockMapper.Setup(m => m.Map<CreateApplicationResponse>(It.IsAny<Application>()))
            .Returns(_fixture.Create<CreateApplicationResponse>());

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser { AppUserId = "test-user", Email = "test@example.com" });

        _mockApplicationRepository.Setup(r => r.AddAsync(It.IsAny<Application>(), default))
            .Callback<Application, CancellationToken>((app, ct) => {

                app.Status.Should().BeFalse();
            })
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateApplication(request);


        _mockApplicationRepository.Verify(r => r.AddAsync(
            It.Is<Application>(a => a.Status == false), default), Times.Once);
    }
}