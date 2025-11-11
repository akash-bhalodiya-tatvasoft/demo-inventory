
using System.Net;
using System.Text.Json;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Services.Interfaces;
using NLog;

namespace Inventory.API.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private static readonly Logger _nlog = LogManager.GetCurrentClassLogger();

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var email = context.User.GetUserEmail();

            var _userService = context.RequestServices.GetRequiredService<IUserService>();

            var user = await _userService.GetUserByEmailAsync(email);

            var logEvent = new LogEventInfo(NLog.LogLevel.Error, _nlog.Name, ex.Message)
            {
                Exception = ex
            };
            logEvent.Properties["CreatedBy"] = user.Id;
            _nlog.Log(logEvent);

            // _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);

            var statusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = ApiResponse<string>.Failure(statusCode, ex.Message);

            var serialized = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(serialized);
        }
    }
}