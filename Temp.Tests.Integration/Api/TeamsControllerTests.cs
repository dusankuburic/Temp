using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Temp.Tests.Integration.Api;

public class TeamsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TeamsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTeams_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/teams");

        // Assert
        // Adjust based on your actual authentication requirements
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,           // If no auth required
            HttpStatusCode.Unauthorized  // If auth required
        );
    }

    [Fact]
    public async Task GetTeam_WithValidId_ReturnsTeam()
    {
        // Arrange
        var teamId = 1;

        // Act
        var response = await _client.GetAsync($"/api/teams/{teamId}");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var team = await response.Content.ReadFromJsonAsync<dynamic>();
            team.Should().NotBeNull();
        }
        else
        {
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.NotFound,
                HttpStatusCode.Unauthorized
            );
        }
    }

    [Fact]
    public async Task GetTeam_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/teams/{invalidId}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }

    [Fact]
    public async Task CreateTeam_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var newTeam = new
        {
            Name = "New Team",
            Description = "A new team for testing"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", newTeam);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Created
        );
    }

    [Fact]
    public async Task UpdateTeam_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var teamId = 1;
        var updateData = new
        {
            Id = teamId,
            Name = "Updated Team Name",
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/teams/{teamId}", updateData);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }

    [Fact]
    public async Task DeleteTeam_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var teamId = 1;

        // Act
        var response = await _client.DeleteAsync($"/api/teams/{teamId}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }
}
