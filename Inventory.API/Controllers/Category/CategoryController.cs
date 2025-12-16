using Inventory.API.Filter;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Category;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(UserRole.SuperAdmin)},{nameof(UserRole.Admin)}")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;
    private readonly IMemoryCacheService _memoryCacheService;

    public CategoryController(ICategoryService categoryService, IUserService userService, IMemoryCacheService memoryCacheService)
    {
        _categoryService = categoryService;
        _userService = userService;
        _memoryCacheService = memoryCacheService;
    }

    [HttpGet]
    [TypeFilter(typeof(ResourceCacheFilter), Arguments = new object[] { "category", 60 * 24, false })]
    public async Task<IActionResult> GetAllAsync([FromQuery] string search)
    {
        var categories = await _categoryService.GetAllAsync(search);
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<Category>>.SuccessResponse(categories, StatusCodes.Status200OK)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<Category>.Failure(StatusCodes.Status404NotFound, "Category not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<Category>.SuccessResponse(category, StatusCodes.Status200OK)
        );
    }

    [HttpPost]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Create })]
    public async Task<IActionResult> CreateAsync([FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var id = await _categoryService.CreateAsync(request, (int)HttpContext.Items["UserId"]);

        await _memoryCacheService.RemoveAsync("category");

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id:int}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Update })]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Invalid request body.", ModelStateHelper.ToErrorResponse(ModelState)));
        }

        var updated = await _categoryService.UpdateAsync(id, request, (int)HttpContext.Items["UserId"]);
        if (!updated)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "Category not found.")
            );
        }

        await _memoryCacheService.RemoveAsync("category");

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpDelete("{id:int}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Delete })]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "Category not found.")
            );
        }

        await _memoryCacheService.RemoveAsync("category");

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }
}
