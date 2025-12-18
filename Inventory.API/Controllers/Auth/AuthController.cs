using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Auth;
using Inventory.Models.Entities;
using Inventory.Services.Auth;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserActivityService _userActivityService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserActivityService userActivityService, IUserService userService)
    {
        _authService = authService;
        _userActivityService = userActivityService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var created = await _authService.RegisterAsync(request);
        if (!created)
            return StatusCode(StatusCodes.Status409Conflict, ApiResponse<string>.Failure(StatusCodes.Status409Conflict, "User already exists."));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<string>.SuccessResponse(
            string.Empty,
            StatusCodes.Status201Created
            ));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var token = await _authService.LoginAsync(request);

        if (token == null)
            return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<string>.Failure(StatusCodes.Status401Unauthorized, "Invalid email or password."));

        var user = await _userService.GetUserByEmailAsync(request.Email);

        await _userActivityService.LogAsync(new UserActivityLog
        {
            UserId = user?.Id ?? 0,
            ActivityType = "Login",
            ActivityDescription = "User logged in successfully.",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ActivityModule = (int)ActivityLogModule.User
        });

        return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse(token, StatusCodes.Status200OK));
    }

    [HttpPost("update-status")]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}")]
    public async Task<IActionResult> UpdateStatus([FromQuery] int userId, [FromQuery] bool status)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        if (userId == null || userId <= 0 || status == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid query params.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.Failure(StatusCodes.Status404NotFound, "User not found.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var id = await _authService.UpdateUserStatusAsync(userId, (int)HttpContext.Items["UserId"], status);

        await _userActivityService.LogAsync(new UserActivityLog
        {
            UserId = user?.Id ?? 0,
            ActivityType = "Status Update",
            ActivityDescription = $"User status updated to {(status ? "Active" : "Inactive")}.",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ActivityModule = (int)ActivityLogModule.User,
            CreatedBy = (int)HttpContext.Items["UserId"]
        });

        return StatusCode(StatusCodes.Status200OK, ApiResponse<int?>.SuccessResponse(id, StatusCodes.Status200OK));
    }
}
