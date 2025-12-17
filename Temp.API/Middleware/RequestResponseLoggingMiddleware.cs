using System.Diagnostics;

namespace Temp.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        // Skip logging for health checks and swagger
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger")) {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try {
            await _next(context);

            stopwatch.Stop();

            var responseBodyText = await ReadResponseBodyAsync(context.Response);

            LogRequest(context, requestBody, stopwatch.ElapsedMilliseconds);
            LogResponse(context, responseBodyText, stopwatch.ElapsedMilliseconds);

            await responseBody.CopyToAsync(originalBodyStream);
        } catch (Exception ex) {
            stopwatch.Stop();
            _logger.LogError(ex, "Request failed after {ElapsedMs}ms: {Method} {Path}",
                stopwatch.ElapsedMilliseconds,
                context.Request.Method,
                context.Request.Path);
            throw;
        } finally {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request) {
        if (!request.ContentLength.HasValue || request.ContentLength == 0)
            return string.Empty;

        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        // Mask sensitive data
        if (body.Contains("password", StringComparison.OrdinalIgnoreCase)) {
            return "[REDACTED - Contains sensitive data]";
        }

        return body.Length > 1000 ? body[..1000] + "..." : body;
    }

    private async Task<string> ReadResponseBodyAsync(HttpResponse response) {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return text.Length > 1000 ? text[..1000] + "..." : text;
    }

    private void LogRequest(HttpContext context, string requestBody, long elapsedMs) {
        _logger.LogInformation(
            "HTTP {Method} {Path} | User: {User} | IP: {IP} | Body: {Body}",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            context.User.Identity?.Name ?? "Anonymous",
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            string.IsNullOrWhiteSpace(requestBody) ? "[Empty]" : requestBody
        );
    }

    private void LogResponse(HttpContext context, string responseBody, long elapsedMs) {
        var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error :
                       context.Response.StatusCode >= 400 ? LogLevel.Warning :
                       LogLevel.Information;

        _logger.Log(logLevel,
            "HTTP {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMs,
            string.IsNullOrWhiteSpace(responseBody) ? "[Empty]" : responseBody
        );
    }
}
