using AutoFixture;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Temp.Database.Repositories;
using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Employees;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Tests.Unit.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggingBroker> _mockLoggingBroker;
    private readonly Mock<IIdentityProvider> _mockIdentityProvider;
    private readonly Mock<IRepository<Employee>> _mockEmployeeRepository;
    private readonly IFixture _fixture;
    private readonly IEmployeeService _service;

    public EmployeeServiceTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggingBroker = new Mock<ILoggingBroker>();
        _mockIdentityProvider = new Mock<IIdentityProvider>();
        _mockEmployeeRepository = new Mock<IRepository<Employee>>();


        _mockUnitOfWork.Setup(uow => uow.Employees).Returns(_mockEmployeeRepository.Object);


        var currentUser = new CurrentUser { Email = "test@example.com", AppUserId = "test-user-id" };
        _mockIdentityProvider.Setup(ip => ip.GetCurrentUser())
            .ReturnsAsync(currentUser);


        _service = new EmployeeService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLoggingBroker.Object,
            _mockIdentityProvider.Object);

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }



    [Fact]
    public async Task CreateEmployee_WithValidData_ReturnsCreatedEmployee() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "Doe")
            .With(r => r.TeamId, 1)
            .Create();

        var mappedEmployee = new Employee
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TeamId = request.TeamId
        };

        var expectedResponse = new CreateEmployeeResponse
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TeamId = request.TeamId
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);

        _mockMapper.Setup(m => m.Map<CreateEmployeeResponse>(It.IsAny<Employee>()))
            .Returns(expectedResponse);

        _mockEmployeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.CreateEmployee(request);


        result.Should().NotBeNull();
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        _mockEmployeeRepository.Verify(r => r.AddAsync(It.Is<Employee>(e =>
            e.FirstName == request.FirstName &&
            e.LastName == request.LastName), default), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_WithEmptyFirstName_ThrowsEmployeeValidationException() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.CreateEmployee(request));

        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_WithEmptyLastName_ThrowsEmployeeValidationException() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.CreateEmployee(request));
    }

    [Fact]
    public async Task CreateEmployee_WithNullMappedEmployee_ThrowsEmployeeServiceException() {

        var request = _fixture.Create<CreateEmployeeRequest>();

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns((Employee)null!);




        await Assert.ThrowsAsync<EmployeeServiceException>(
            async () => await _service.CreateEmployee(request));
    }

    [Fact]
    public async Task CreateEmployee_WhenDatabaseFails_ThrowsEmployeeServiceException() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);

        _mockEmployeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .ThrowsAsync(new Exception("Database connection failed"));


        await Assert.ThrowsAsync<EmployeeServiceException>(
            async () => await _service.CreateEmployee(request));

        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_SetsAuditableInfoOnCreate() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var expectedResponse = new CreateEmployeeResponse
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);

        _mockMapper.Setup(m => m.Map<CreateEmployeeResponse>(It.IsAny<Employee>()))
            .Returns(expectedResponse);

        _mockEmployeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEmployee(request);


        _mockIdentityProvider.Verify(ip => ip.GetCurrentUser(), Times.Once);
    }

    [Fact]
    public async Task GetEmployee_WithValidId_CallsRepositoryQueryNoTracking() {

        var employeeId = 1;
        var employee = new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" };
        var employees = new List<Employee> { employee };
        var mockQueryable = employees.AsQueryable().BuildMockDbSet().Object;

        _mockEmployeeRepository.Setup(r => r.QueryNoTracking())
            .Returns(mockQueryable);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, GetEmployeeResponse>()));


        var result = await _service.GetEmployee(employeeId);


        result.Should().NotBeNull();
        result.Id.Should().Be(employeeId);
        result.FirstName.Should().Be("John");
        _mockEmployeeRepository.Verify(r => r.QueryNoTracking(), Times.Once);
    }

    [Fact]
    public async Task GetEmployee_WithNonExistentId_ThrowsEmployeeValidationException() {

        var nonExistentId = 999;
        var employees = new List<Employee>().AsQueryable().BuildMockDbSet().Object;

        _mockEmployeeRepository.Setup(r => r.QueryNoTracking())
            .Returns(employees);

        _mockMapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, GetEmployeeResponse>()));



        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.GetEmployee(nonExistentId));
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_ReturnsSuccess() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .With(e => e.FirstName, "John")
            .With(e => e.LastName, "Doe")
            .Create();

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.DeleteEmployeeAsync(employeeId);


        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockEmployeeRepository.Verify(r => r.Remove(existingEmployee), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployee_WithNonExistentId_ThrowsEmployeeValidationException() {

        var nonExistentId = 999;

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(nonExistentId, default))
            .ReturnsAsync((Employee?)null);


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.DeleteEmployeeAsync(nonExistentId));

        _mockEmployeeRepository.Verify(r => r.Remove(It.IsAny<Employee>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task DeleteEmployee_WhenDatabaseFails_ThrowsEmployeeServiceException() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        await Assert.ThrowsAsync<EmployeeServiceException>(
            async () => await _service.DeleteEmployeeAsync(employeeId));
    }

    [Fact]
    public async Task DeleteEmployee_LogsErrorOnFailure() {

        var nonExistentId = 999;

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(nonExistentId, default))
            .ReturnsAsync((Employee?)null);


        try {
            await _service.DeleteEmployeeAsync(nonExistentId);
        } catch (EmployeeValidationException) {

        }


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<EmployeeValidationException>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_WithValidData_ReturnsSuccess() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .With(e => e.FirstName, "John")
            .With(e => e.LastName, "Doe")
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee))
            .Callback<UpdateEmployeeRequest, Employee>((req, emp) => {
                emp.FirstName = req.FirstName;
                emp.LastName = req.LastName;
            });

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        var result = await _service.UpdateEmployee(updateRequest);


        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockEmployeeRepository.Verify(r => r.Update(existingEmployee), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_WithEmptyFirstName_ThrowsEmployeeValidationException() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "")
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee))
            .Callback<UpdateEmployeeRequest, Employee>((req, emp) => {
                emp.FirstName = req.FirstName;
                emp.LastName = req.LastName;
            });


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.UpdateEmployee(updateRequest));
    }

    [Fact]
    public async Task UpdateEmployee_WithEmptyLastName_ThrowsEmployeeValidationException() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, "")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee))
            .Callback<UpdateEmployeeRequest, Employee>((req, emp) => {
                emp.FirstName = req.FirstName;
                emp.LastName = req.LastName;
            });


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.UpdateEmployee(updateRequest));
    }

    [Fact]
    public async Task UpdateEmployee_SetsAuditableInfoOnUpdate() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .With(e => e.FirstName, "John")
            .With(e => e.LastName, "Doe")
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateEmployee(updateRequest);


        _mockIdentityProvider.Verify(ip => ip.GetCurrentUser(), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_WhenDatabaseFails_ThrowsEmployeeServiceException() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .With(e => e.FirstName, "John")
            .With(e => e.LastName, "Doe")
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ThrowsAsync(new Exception("Database error"));


        await Assert.ThrowsAsync<EmployeeServiceException>(
            async () => await _service.UpdateEmployee(updateRequest));
    }


    [Fact]
    public async Task CreateEmployee_OnSuccess_DoesNotLogError() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var expectedResponse = new CreateEmployeeResponse
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);

        _mockMapper.Setup(m => m.Map<CreateEmployeeResponse>(It.IsAny<Employee>()))
            .Returns(expectedResponse);

        _mockEmployeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEmployee(request);


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEmployee_OnSuccess_DoesNotLogError() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.DeleteEmployeeAsync(employeeId);


        _mockLoggingBroker.Verify(l => l.LogError(It.IsAny<Exception>()), Times.Never);
        _mockLoggingBroker.Verify(l => l.LogCritical(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task CreateEmployee_WithWhitespaceFirstName_ThrowsEmployeeValidationException() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "   ")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.CreateEmployee(request));
    }

    [Fact]
    public async Task CreateEmployee_WithWhitespaceLastName_ThrowsEmployeeValidationException() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "   ")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.CreateEmployee(request));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateEmployee_WithInvalidFirstName_ThrowsEmployeeValidationException(string? invalidFirstName) {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, invalidFirstName!)
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee))
            .Callback<UpdateEmployeeRequest, Employee>((req, emp) => {
                emp.FirstName = req.FirstName;
                emp.LastName = req.LastName;
            });


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.UpdateEmployee(updateRequest));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateEmployee_WithInvalidLastName_ThrowsEmployeeValidationException(string? invalidLastName) {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, invalidLastName!)
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee))
            .Callback<UpdateEmployeeRequest, Employee>((req, emp) => {
                emp.FirstName = req.FirstName;
                emp.LastName = req.LastName;
            });


        await Assert.ThrowsAsync<EmployeeValidationException>(
            async () => await _service.UpdateEmployee(updateRequest));
    }

    [Fact]
    public async Task CreateEmployee_CallsRepositoryAddAsync() {

        var request = _fixture.Build<CreateEmployeeRequest>()
            .With(r => r.FirstName, "John")
            .With(r => r.LastName, "Doe")
            .Create();

        var mappedEmployee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var expectedResponse = new CreateEmployeeResponse
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _mockMapper.Setup(m => m.Map<Employee>(request))
            .Returns(mappedEmployee);

        _mockMapper.Setup(m => m.Map<CreateEmployeeResponse>(It.IsAny<Employee>()))
            .Returns(expectedResponse);

        _mockEmployeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.CreateEmployee(request);


        _mockEmployeeRepository.Verify(r => r.AddAsync(It.IsAny<Employee>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployee_CallsRepositoryRemove() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .Create();

        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(employeeId, default))
            .ReturnsAsync(existingEmployee);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.DeleteEmployeeAsync(employeeId);


        _mockEmployeeRepository.Verify(r => r.Remove(existingEmployee), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployee_CallsRepositoryUpdate() {

        var employeeId = 1;
        var existingEmployee = _fixture.Build<Employee>()
            .With(e => e.Id, employeeId)
            .With(e => e.FirstName, "John")
            .With(e => e.LastName, "Doe")
            .Create();

        var updateRequest = _fixture.Build<UpdateEmployeeRequest>()
            .With(r => r.Id, employeeId)
            .With(r => r.FirstName, "Jane")
            .With(r => r.LastName, "Smith")
            .Create();

        _mockEmployeeRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), default))
            .ReturnsAsync(existingEmployee);

        _mockMapper.Setup(m => m.Map(updateRequest, existingEmployee));

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
            .ReturnsAsync(1);


        await _service.UpdateEmployee(updateRequest);

        _mockEmployeeRepository.Verify(r => r.Update(existingEmployee), Times.Once);
    }
}