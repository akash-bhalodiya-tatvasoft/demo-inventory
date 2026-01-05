using Inventory.Common.Responses;
using Inventory.Models.Dashboard;
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
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpPost]
    public async Task<IActionResult> GetDashboard([FromBody] DashboardRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.")
            );
        }

        var data = await _dashboardService.GetDashboardAsync(request);

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<DashboardResponse>.SuccessResponse(data, StatusCodes.Status200OK)
        );
    }
}
