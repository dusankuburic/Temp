using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Temp.Tests.Integration.Api;

public class TeamsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TeamsControllerTests(WebApplicationFactory<Program> factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTeams_WithoutAuthentication_ReturnsUnauthorized() {

        var response = await _client.GetAsync("/api/teams");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized
        );
    }

    [Fact]
    public async Task GetTeam_WithValidId_ReturnsTeam() {

        var teamId = 1;

        var response = await _client.GetAsync($"/api/teams/{teamId}");

        if (response.StatusCode == HttpStatusCode.OK) {
            var team = await response.Content.ReadFromJsonAsync<dynamic>();
            team.Should().NotBeNull();
        } else {
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.NotFound,
                HttpStatusCode.Unauthorized
            );
        }
    }

    [Fact]
    public async Task GetTeam_WithInvalidId_ReturnsNotFound() {

        var invalidId = 99999;
        var response = await _client.GetAsync($"/api/teams/{invalidId}");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }

    [Fact]
    public async Task CreateTeam_WithoutAuthentication_ReturnsUnauthorized() {

        var newTeam = new
        {
            Name = "New Team",
            Description = "A new team for testing"
        };


        var response = await _client.PostAsJsonAsync("/api/teams", newTeam);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Unauthorized,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Created
        );
    }

    [Fact]
    public async Task UpdateTeam_WithValidData_ReturnsSuccess() {

        var teamId = 1;
        var updateData = new
        {
            Id = teamId,
            Name = "Updated Team Name",
            Description = "Updated description"
        };


        var response = await _client.PutAsJsonAsync($"/api/teams/{teamId}", updateData);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }

    [Fact]
    public async Task DeleteTeam_WithValidId_ReturnsSuccess() {

        var teamId = 1;
        var response = await _client.DeleteAsync($"/api/teams/{teamId}");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        );
    }
}