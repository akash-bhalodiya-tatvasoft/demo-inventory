using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Auth;
using Inventory.Models.Entities;
using Inventory.Services.Auth;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}
