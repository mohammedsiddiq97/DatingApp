public class MyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MyMiddleware> _logger;

    public MyMiddleware(RequestDelegate next, ILogger<MyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Processing request: {RequestPath}", context.Request.Path);
        _logger.LogInformation("Request Method: {Method}", context.Request.Method);
        _logger.LogInformation("Request Headers: {Headers}", context.Request.Headers);

        await _next(context); // Call the next middleware

        _logger.LogInformation("Finished processing request: {RequestPath}", context.Request.Path);
    }
}
