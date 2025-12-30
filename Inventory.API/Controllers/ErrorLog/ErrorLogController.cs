using Inventory.Common.Helpers;
using Inventory.Common.Requests;
using Inventory.Common.Responses;
using Inventory.Models.Entities;
using Inventory.Models.ErrorLog;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ErrorLogController : ControllerBase
{
    private readonly IErrorLogService _errorLogService;

    public ErrorLogController(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    [HttpPost]
    public async Task<IActionResult> GetErrorLogsAsync([FromBody] ErrorLogFilterRequest request)
    {
        var result = await _errorLogService.GetAllAsync(request);

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<PaginatedResponse<ErrorLog>>
                .SuccessResponse(result, StatusCodes.Status200OK)
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetErrorLogByIdAsync(string id)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }

        var log = await _errorLogService.GetByIdAsync(convertedId);
        if (log == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<ErrorLog>.Failure(StatusCodes.Status404NotFound, "Error log not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<ErrorLog>.SuccessResponse(log, StatusCodes.Status200OK)
        );
    }
}
