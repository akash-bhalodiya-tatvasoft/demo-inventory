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
[ApiVersion("1.0")]
[ApiVersion("2.0")]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }
        var profile = await _profileService.GetByIdAsync(convertedId);
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
            ApiResponse<string>.SuccessResponse(EncryptionHelper.EncryptId(id.Value.ToString()), StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Update })]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UserProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }
        var updated = await _profileService.UpdateAsync(convertedId, request, (int)HttpContext.Items["UserId"]);
        if (!updated)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "User profile not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<string>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpDelete("{id}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Delete })]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }
        var deleted = await _profileService.DeleteAsync(convertedId);
        if (!deleted)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "User profile not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<string>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }
}
