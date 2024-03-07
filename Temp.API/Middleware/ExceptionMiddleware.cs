namespace Temp.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        IHostEnvironment env,
        ILogger<ExceptionMiddleware> logger) {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception exception) {
            _logger.LogError(exception, exception.Message);

            var response = !_env.IsDevelopment()
                ? $"{exception.Message}\n{exception.StackTrace}"
                : "";

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.AddApplicationError(response);

            await context.Response.WriteAsync(response);
        }
    }
}
