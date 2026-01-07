using Inventory.API.Filter;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Entities;
using Inventory.Models.Product;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    public ProductController(IProductService productService, IUserService userService)
    {
        _productService = productService;
        _userService = userService;

    }

    [HttpGet]
    [EnableRateLimiting("Fixed")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAllAsyncV1()
    {
        var products = await _productService.GetAllAsync("");
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(products, StatusCodes.Status200OK)
        );
    }

    [HttpGet]
    [EnableRateLimiting("Fixed")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetAllAsyncV2([FromQuery] string search)
    {
        var products = await _productService.GetAllAsync(search);
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(products, StatusCodes.Status200OK)
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
        var product = await _productService.GetByIdAsync(convertedId);
        if (product == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<ProductResponse>.Failure(StatusCodes.Status404NotFound, "Product not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<ProductResponse>.SuccessResponse(product, StatusCodes.Status200OK)
        );
    }

    [HttpPost]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Create })]
    public async Task<IActionResult> CreateAsync([FromForm] ProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                ApiResponse<string>.Failure(
                    StatusCodes.Status400BadRequest,
                    "Invalid request body.",
                    ModelStateHelper.ToErrorResponse(ModelState)
                )
            );
        }

        var id = await _productService.CreateAsync(request, (int)HttpContext.Items["UserId"]);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<string>.SuccessResponse(EncryptionHelper.EncryptId(id.ToString()), StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Update })]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] ProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                ApiResponse<string>.Failure(
                    StatusCodes.Status400BadRequest,
                    "Invalid request body.",
                    ModelStateHelper.ToErrorResponse(ModelState)
                )
            );
        }

        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }
        var updated = await _productService.UpdateAsync(convertedId, request, (int)HttpContext.Items["UserId"]);
        if (!updated)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "Product not found.")
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
        var deleted = await _productService.DeleteAsync(convertedId);
        if (!deleted)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<int>.Failure(StatusCodes.Status404NotFound, "Product not found.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<string>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpPut("offer/{productId:int}")]
    [TypeFilter(typeof(PermissionFilter), Arguments = new object[] { OperationType.Update })]
    public async Task<IActionResult> AddOfferAsync(int productId, [FromBody] ProductOfferRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(
                StatusCodes.Status400BadRequest,
                ApiResponse<string>.Failure(
                    StatusCodes.Status400BadRequest,
                    "Invalid request body.",
                    ModelStateHelper.ToErrorResponse(ModelState)
                )
            );
        }

        await _productService.AddOfferAsync(productId, request, (int)HttpContext.Items["UserId"]);

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(productId, StatusCodes.Status200OK)
        );
    }
}
