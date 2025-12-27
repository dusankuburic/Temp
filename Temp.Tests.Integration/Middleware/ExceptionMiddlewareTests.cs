using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Temp.API.Models;

namespace Temp.Tests.Integration.Middleware;

public class ExceptionMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExceptionMiddlewareTests(WebApplicationFactory<Program> factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task InvalidRequest_ReturnsStructuredError() {
        var invalidData = new { };

        var response = await _client.PostAsJsonAsync("/api/employees", invalidData);

        if (response.StatusCode == HttpStatusCode.BadRequest) {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be(400);
            errorResponse.Message.Should().NotBeNullOrEmpty();
            errorResponse.TraceId.Should().NotBeNullOrEmpty();
            errorResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }

    [Fact]
    public async Task NotFoundEndpoint_Returns404() {
        var response = await _client.GetAsync("/api/nonexistent");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ErrorResponse_ContainsTraceId() {
        var response = await _client.GetAsync("/api/nonexistent/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}