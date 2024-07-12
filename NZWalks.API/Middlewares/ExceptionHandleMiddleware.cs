using System.Net;

namespace NZWalks.API.Middlewares;

public class ExceptionHandleMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandleMiddleware> logger;

    public ExceptionHandleMiddleware(
        ILogger<ExceptionHandleMiddleware> logger,
        RequestDelegate next)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid();

            logger.LogError(ex, $"{errorId}: {ex.Message}");

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = new
            {
                Id = errorId,
                ErrorMessage = "Something went wrong! We are trying to solve this problem..."
            };

            await httpContext.Response.WriteAsJsonAsync(error);
        }
    }
}
