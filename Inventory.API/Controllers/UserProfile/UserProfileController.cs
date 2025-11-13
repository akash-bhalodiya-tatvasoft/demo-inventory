using System.Security.Claims;
using Inventory.API.Filter;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Entities;
using Inventory.Models.UserProfiles;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _profileService;
    private readonly IUserService _userService;

    public UserProfileController(IUserProfileService profileService, IUserService userService)
    {
        _profileService = profileService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] string search)
    {
        var profiles = await _profileService.GetAllAsync(search);
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<UserProfileResponse>>.SuccessResponse(profiles, StatusCodes.Status200OK)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var profile = await _profileService.GetByIdAsync(id);
        if (profile == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<UserProfileResponse>.Failure(StatusCodes.Status404NotFound, "User profile not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<UserProfileResponse>.SuccessResponse(profile, StatusCodes.Status200OK)
        );
    }

    [HttpPost]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Create })]
    public async Task<IActionResult> CreateAsync([FromBody] UserProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var id = await _profileService.CreateAsync(request, (int)HttpContext.Items["UserId"]);

        if (id == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<UserProfile>.Failure(StatusCodes.Status404NotFound, "User not found.")
            );
        }


        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.SuccessResponse(id.Value, StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id:int}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Update })]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var updated = await _profileService.UpdateAsync(id, request, (int)HttpContext.Items["UserId"]);
        if (!updated)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "User profile not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpDelete("{id:int}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Delete })]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _profileService.DeleteAsync(id);
        if (!deleted)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "User profile not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }
}
