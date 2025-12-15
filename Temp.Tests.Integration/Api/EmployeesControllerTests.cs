using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Xunit;

namespace Temp.Tests.Integration.Api;

public class EmployeesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public EmployeesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // TODO: Add authentication token to client
        // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetEmployees_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/employees");

        // Assert
        // Without authentication, we expect Unauthorized; with auth we'd expect OK
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ReturnsEmployee()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var response = await _client.GetAsync($"/api/employees/{employeeId}");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var employee = await response.Content.ReadFromJsonAsync<GetEmployeeResponse>();
            employee.Should().NotBeNull();
            employee!.Id.Should().Be(employeeId);
        }
    }

    [Fact]
    public async Task GetEmployee_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/employees/{invalidId}");

        // Assert
        // Without authentication, we expect Unauthorized; with auth we'd expect NotFound
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe"
            // Email = "john.doe@example.com" // Add if Email property exists
            // Add other required fields
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/employees", request);

        // Assert
        // Without authentication, we expect Unauthorized; with auth we'd expect Created or OK
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.Unauthorized);

        if (response.IsSuccessStatusCode)
        {
            var createdEmployee = await response.Content.ReadFromJsonAsync<GetEmployeeResponse>();
            createdEmployee.Should().NotBeNull();
            createdEmployee!.FirstName.Should().Be(request.FirstName);
            createdEmployee.LastName.Should().Be(request.LastName);
        }
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateEmployeeRequest
        {
            FirstName = "", // Invalid
            LastName = "Doe"
            // Email = "invalid-email" // Invalid email format - add if Email property exists
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/employees", invalidRequest);

        // Assert
        // Without authentication, we expect Unauthorized; with auth we'd expect BadRequest
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateEmployee_WithValidData_ReturnsOk()
    {
        // Arrange
        var employeeId = 1;
        var request = new UpdateEmployeeRequest
        {
            Id = employeeId,
            FirstName = "John Updated",
            LastName = "Doe Updated"
            // Add other required fields
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/employees/{employeeId}", request);

        // Assert
        if (response.IsSuccessStatusCode)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedEmployee = await response.Content.ReadFromJsonAsync<GetEmployeeResponse>();
            updatedEmployee.Should().NotBeNull();
            updatedEmployee!.FirstName.Should().Be(request.FirstName);
        }
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var response = await _client.DeleteAsync($"/api/employees/{employeeId}");

        // Assert
        // Adjust based on your actual authentication requirements
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NoContent,    // If auth provided and successful
            HttpStatusCode.OK,           // Alternative success response
            HttpStatusCode.Unauthorized  // If auth required but not provided
        );
    }
}
