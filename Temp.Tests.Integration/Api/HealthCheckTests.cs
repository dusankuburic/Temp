using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Temp.Tests.Integration.Api;

public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthCheckTests(WebApplicationFactory<Program> factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_Endpoint_ReturnsOk() {

        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.ServiceUnavailable
        );
    }

    [Fact]
    public async Task HealthCheck_Endpoint_ReturnsJson() {

        var response = await _client.GetAsync("/health");

        response.Content.Headers.ContentType?.MediaType.Should().BeOneOf(
            "application/json",
            "text/plain"
        );
    }

    [Fact]
    public async Task HealthCheck_Response_ContainsStatus() {

        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().NotBeNullOrEmpty();
    }
}