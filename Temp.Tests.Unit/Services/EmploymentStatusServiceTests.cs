using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses;
using Temp.Services.EmploymentStatuses.Exceptions;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class EmploymentStatusServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<EmploymentStatus>> _mockEmploymentStatusRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IEmploymentStatusService _service;

    public EmploymentStatusServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmploymentStatusRepository = new Mock<IRepository<EmploymentStatus>>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _mockUnitOfWork.Setup(uow => uow.EmploymentStatuses)
            .Returns(_mockEmploymentStatusRepository.Object);

        _mockIdentityProvider.Setup(i => i.GetCurrentUser())
            .ReturnsAsync(new CurrentUser {
                AppUserId = "test-user",
                Email = "test@example.com"
            });

        _service = new EmploymentStatusService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);
    }



    [Fact]
    public async Task CreateEmploymentStatus_WithValidRequest_ReturnsCreatedResponse() {

        var request = new CreateEmploymentStatusRequest { Name = "Full-Time" };
        var employmentStatus = new EmploymentStatus {
            Id = 1,
            Name = "Full-Time",
            IsActive = true
        };
        var response = new CreateEmploymentStatusResponse { Id = 1 };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(employmentStatus);

        _mockMapper.Setup(m => m.Map<CreateEmploymentStatusResponse>(It.IsAny<EmploymentStatus>()))
            .Returns(response);

        _mockEmploymentStatusRepository.Setup(r => r.AddAsync(It.IsAny<EmploymentStatus>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateEmploymentStatus(request);


        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mockEmploymentStatusRepository.Verify(r => r.AddAsync(It.IsAny<EmploymentStatus>(), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateEmploymentStatus_WithEmptyName_ThrowsEmploymentStatusValidationException() {

        var request = new CreateEmploymentStatusRequest { Name = "" };
        var employmentStatus = new EmploymentStatus { Name = "" };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(employmentStatus);


        Func<Task> act = () => _service.CreateEmploymentStatus(request);


        await act.Should().ThrowAsync<EmploymentStatusValidationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task CreateEmploymentStatus_WithInvalidNames_ThrowsEmploymentStatusValidationException(string invalidName) {

        var request = new CreateEmploymentStatusRequest { Name = invalidName };
        var employmentStatus = new EmploymentStatus { Name = invalidName };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(employmentStatus);


        Func<Task> act = () => _service.CreateEmploymentStatus(request);


        await act.Should().ThrowAsync<EmploymentStatusValidationException>();
    }

    [Fact]
    public async Task CreateEmploymentStatus_WithNullMappedObject_ThrowsEmploymentStatusServiceException() {

        var request = new CreateEmploymentStatusRequest { Name = "Test" };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns((EmploymentStatus)null);


        Func<Task> act = () => _service.CreateEmploymentStatus(request);


        await act.Should().ThrowAsync<EmploymentStatusServiceException>();
    }

    [Fact]
    public async Task CreateEmploymentStatus_SetsAuditInfo() {

        var request = new CreateEmploymentStatusRequest { Name = "Part-Time" };
        EmploymentStatus capturedStatus = null;

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(new EmploymentStatus { Name = "Part-Time" });

        _mockMapper.Setup(m => m.Map<CreateEmploymentStatusResponse>(It.IsAny<EmploymentStatus>()))
            .Returns(new CreateEmploymentStatusResponse { Id = 1 });

        _mockEmploymentStatusRepository.Setup(r => r.AddAsync(It.IsAny<EmploymentStatus>(), default))
            .Callback<EmploymentStatus, CancellationToken>((status, _) => capturedStatus = status)
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEmploymentStatus(request);


        capturedStatus.Should().NotBeNull();
        capturedStatus.CreatedBy.Should().Be("test-user");
    }





    [Fact]
    public async Task GetEmploymentStatus_WithValidId_ReturnsEmploymentStatus() {

        var request = new GetEmploymentStatusRequest { Id = 1 };
        var employmentStatus = new EmploymentStatus {
            Id = 1,
            Name = "Full-Time",
            IsActive = true
        };
        var response = new GetEmploymentStatusResponse {
            Id = 1,
            Name = "Full-Time"
        };

        var queryable = new List<EmploymentStatus> { employmentStatus }
            .AsQueryable()
            .AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatus(request);


        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmploymentStatus_WithInactiveStatus_ReturnsNull() {

        var request = new GetEmploymentStatusRequest { Id = 1 };
        var employmentStatus = new EmploymentStatus {
            Id = 1,
            Name = "Inactive Status",
            IsActive = false
        };

        var queryable = new List<EmploymentStatus> { employmentStatus }
            .AsQueryable()
            .AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatus(request);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEmploymentStatus_WithNonExistentId_ReturnsNull() {

        var request = new GetEmploymentStatusRequest { Id = 999 };

        var queryable = new List<EmploymentStatus>()
            .AsQueryable()
            .AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatus(request);


        result.Should().BeNull();
    }





    [Fact]
    public async Task GetEmploymentStatuses_ReturnsOnlyActiveStatuses() {

        var statuses = new List<EmploymentStatus> {
            new EmploymentStatus { Id = 1, Name = "Full-Time", IsActive = true },
            new EmploymentStatus { Id = 2, Name = "Part-Time", IsActive = true },
            new EmploymentStatus { Id = 3, Name = "Inactive", IsActive = false }
        };

        var queryable = statuses.AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatuses();


        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEmploymentStatuses_ReturnsEmptyListWhenNoStatuses() {

        var queryable = new List<EmploymentStatus>()
            .AsQueryable()
            .AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatuses();


        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEmploymentStatuses_ReturnsOrderedByName() {

        var statuses = new List<EmploymentStatus> {
            new EmploymentStatus { Id = 1, Name = "Zzz-Last", IsActive = true },
            new EmploymentStatus { Id = 2, Name = "Aaa-First", IsActive = true },
            new EmploymentStatus { Id = 3, Name = "Mmm-Middle", IsActive = true }
        };

        var queryable = statuses.AsQueryable().BuildMockDbSet().Object;

        _mockEmploymentStatusRepository.Setup(r => r.QueryNoTracking())
            .Returns(queryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<EmploymentStatus, GetEmploymentStatusResponse>()));


        var result = await _service.GetEmploymentStatuses();


        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Aaa-First");
        result[1].Name.Should().Be("Mmm-Middle");
        result[2].Name.Should().Be("Zzz-Last");
    }





    [Fact]
    public async Task UpdateEmploymentStatus_WithValidRequest_UpdatesStatus() {

        var request = new UpdateEmploymentStatusRequest { Id = 1, Name = "Updated Name" };
        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Original Name",
            IsActive = true
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateEmplymentStatus(request);


        result.Should().NotBeNull();
        _mockEmploymentStatusRepository.Verify(r => r.Update(It.Is<EmploymentStatus>(s =>
            s.Name == "Updated Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateEmploymentStatus_WithEmptyName_ThrowsEmploymentStatusValidationException() {

        var request = new UpdateEmploymentStatusRequest { Id = 1, Name = "" };
        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Original",
            IsActive = true
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);


        Func<Task> act = () => _service.UpdateEmplymentStatus(request);


        await act.Should().ThrowAsync<EmploymentStatusValidationException>();
    }

    [Fact]
    public async Task UpdateEmploymentStatus_SetsAuditInfo() {

        var request = new UpdateEmploymentStatusRequest { Id = 1, Name = "Updated" };
        EmploymentStatus capturedStatus = null;

        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Original",
            IsActive = true,
            CreatedBy = "original@example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockEmploymentStatusRepository.Setup(r => r.Update(It.IsAny<EmploymentStatus>()))
            .Callback<EmploymentStatus>(status => capturedStatus = status);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateEmplymentStatus(request);


        capturedStatus.Should().NotBeNull();
        capturedStatus.UpdatedBy.Should().Be("test-user");
    }





    [Fact]
    public async Task UpdateEmploymentStatusStatus_TogglesActiveToInactive() {

        var request = new UpdateEmploymentStatusStatusRequest { Id = 1 };
        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Test Status",
            IsActive = true
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateEmploymentStatusStatus(request);


        result.Should().NotBeNull();
        _mockEmploymentStatusRepository.Verify(r => r.Update(It.Is<EmploymentStatus>(s =>
            s.IsActive == false)), Times.Once);
    }

    [Fact]
    public async Task UpdateEmploymentStatusStatus_TogglesInactiveToActive() {

        var request = new UpdateEmploymentStatusStatusRequest { Id = 1 };
        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Test Status",
            IsActive = false
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateEmploymentStatusStatus(request);


        result.Should().NotBeNull();
        _mockEmploymentStatusRepository.Verify(r => r.Update(It.Is<EmploymentStatus>(s =>
            s.IsActive == true)), Times.Once);
    }

    [Fact]
    public async Task UpdateEmploymentStatusStatus_SetsAuditInfo() {

        var request = new UpdateEmploymentStatusStatusRequest { Id = 1 };
        EmploymentStatus capturedStatus = null;

        var existingStatus = new EmploymentStatus {
            Id = 1,
            Name = "Test",
            IsActive = true
        };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockEmploymentStatusRepository.Setup(r => r.Update(It.IsAny<EmploymentStatus>()))
            .Callback<EmploymentStatus>(status => capturedStatus = status);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateEmploymentStatusStatus(request);


        capturedStatus.Should().NotBeNull();
        capturedStatus.UpdatedBy.Should().Be("test-user");
    }





    [Fact]
    public async Task EmploymentStatusExists_WithExistingName_ReturnsTrue() {

        var name = "Full-Time";

        _mockEmploymentStatusRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(true);


        var result = await _service.EmploymentStatusExists(name);


        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmploymentStatusExists_WithNonExistingName_ReturnsFalse() {

        var name = "Non-Existent";

        _mockEmploymentStatusRepository.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(false);


        var result = await _service.EmploymentStatusExists(name);


        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Full-Time")]
    [InlineData("Part-Time")]
    [InlineData("Contract")]
    [InlineData("Intern")]
    public async Task EmploymentStatusExists_WithVariousNames_ChecksCorrectly(string name) {

        _mockEmploymentStatusRepository.Setup(r => r.AnyAsync(
            It.Is<Expression<Func<EmploymentStatus, bool>>>(expr => true), default))
            .ReturnsAsync(true);


        var result = await _service.EmploymentStatusExists(name);


        result.Should().BeTrue();
        _mockEmploymentStatusRepository.Verify(r => r.AnyAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default), Times.Once);
    }





    [Fact]
    public async Task CreateEmploymentStatus_WithInvalidName_LogsError() {

        var request = new CreateEmploymentStatusRequest { Name = "" };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(new EmploymentStatus { Name = "" });


        try {
            await _service.CreateEmploymentStatus(request);
        } catch (EmploymentStatusValidationException) {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateEmploymentStatus_WithNullMapped_LogsError() {

        var request = new CreateEmploymentStatusRequest { Name = "Test" };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns((EmploymentStatus)null);


        try {
            await _service.CreateEmploymentStatus(request);
        } catch (EmploymentStatusServiceException) {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }





    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance() {

        var service = new EmploymentStatusService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);


        service.Should().NotBeNull();
    }





    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("This is a very long employment status name that might be at the edge of what the system allows")]
    public async Task CreateEmploymentStatus_WithVariousNameLengths_HandlesCorrectly(string name) {

        var request = new CreateEmploymentStatusRequest { Name = name };
        var status = new EmploymentStatus { Name = name };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(status);

        _mockMapper.Setup(m => m.Map<CreateEmploymentStatusResponse>(It.IsAny<EmploymentStatus>()))
            .Returns(new CreateEmploymentStatusResponse { Id = 1 });

        _mockEmploymentStatusRepository.Setup(r => r.AddAsync(It.IsAny<EmploymentStatus>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateEmploymentStatus(request);


        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateEmploymentStatus_VerifiesRepositoryAddIsCalled() {

        var request = new CreateEmploymentStatusRequest { Name = "Test" };

        _mockMapper.Setup(m => m.Map<EmploymentStatus>(request))
            .Returns(new EmploymentStatus { Name = "Test" });

        _mockMapper.Setup(m => m.Map<CreateEmploymentStatusResponse>(It.IsAny<EmploymentStatus>()))
            .Returns(new CreateEmploymentStatusResponse { Id = 1 });

        _mockEmploymentStatusRepository.Setup(r => r.AddAsync(It.IsAny<EmploymentStatus>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEmploymentStatus(request);


        _mockEmploymentStatusRepository.Verify(r => r.AddAsync(
            It.Is<EmploymentStatus>(s => s.Name == "Test"), default), Times.Once);
    }

    [Fact]
    public async Task UpdateEmploymentStatus_VerifiesRepositoryUpdateIsCalled() {

        var request = new UpdateEmploymentStatusRequest { Id = 1, Name = "Updated" };
        var existingStatus = new EmploymentStatus { Id = 1, Name = "Original", IsActive = true };

        _mockEmploymentStatusRepository.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<EmploymentStatus, bool>>>(), default))
            .ReturnsAsync(existingStatus);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateEmplymentStatus(request);


        _mockEmploymentStatusRepository.Verify(r => r.Update(
            It.Is<EmploymentStatus>(s => s.Id == 1 && s.Name == "Updated")), Times.Once);
    }


}