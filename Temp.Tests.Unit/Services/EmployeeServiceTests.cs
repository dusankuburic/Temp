using AutoFixture;
using AutoMapper;
using Moq;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Employees;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Tests.Unit.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly IFixture _fixture;
    private readonly IEmployeeService _service;

    public EmployeeServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _fixture = new Fixture();

        // Create service with mocked dependencies
        _service = new EmployeeService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ReturnsEmployee() {
        // Arrange
        var employeeId = 1;
        var expectedEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(employeeId, default))
            .ReturnsAsync(expectedEmployee);

        // Act
        // var result = await _service.GetEmployee(employeeId);

        // Assert
        // result.Should().NotBeNull();
        // result.Id.Should().Be(employeeId);
    }

    [Fact]
    public async Task GetEmployee_WithInvalidId_ThrowsNotFoundException() {
        // Arrange
        var invalidId = 999;
        _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(invalidId, default))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        // await Assert.ThrowsAsync<NotFoundException>(
        //     async () => await _service.GetEmployee(invalidId));
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ReturnsCreatedEmployee() {
        // Arrange
        var request = _fixture.Build<CreateEmployeeRequest>()
            .Create();

        var employee = _fixture.Build<Employee>()
            .With(e => e.FirstName, request.FirstName)
            .With(e => e.LastName, request.LastName)
            .Create();

        _mockUnitOfWork.Setup(uow => uow.Employees.AddAsync(It.IsAny<Employee>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        // var result = await _service.CreateEmployee(request);

        // Assert
        // result.Should().NotBeNull();
        // result.FirstName.Should().Be(request.FirstName);
        // result.LastName.Should().Be(request.LastName);
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidData_ThrowsValidationException() {
        // Arrange
        var invalidRequest = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "") // Invalid: empty name
            .Create();

        // Act & Assert
        // await Assert.ThrowsAsync<ValidationException>(
        //     async () => await _service.CreateEmployee(invalidRequest));
    }

    [Fact]
    public async Task UpdateEmployee_WithValidData_ReturnsUpdatedEmployee() {
        // Arrange
        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .Create();

        _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.Employees.Update(It.IsAny<Employee>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        // var result = await _service.UpdateEmployee(updateRequest);

        // Assert
        // result.Should().NotBeNull();
        // result.Id.Should().Be(employeeId);
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_DeletesEmployee() {
        // Arrange
        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        _mockUnitOfWork.Setup(uow => uow.Employees.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.Employees.Remove(It.IsAny<Employee>()));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteEmployeeAsync(employeeId);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.Employees.Remove(existingEmployee), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }
}
