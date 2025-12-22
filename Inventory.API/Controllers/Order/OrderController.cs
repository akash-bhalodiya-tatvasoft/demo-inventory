using Inventory.API.Filter;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Order;
using Inventory.Models.Entities;
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
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public OrderController(IOrderService orderService, IProductService productService)
    {
        _orderService = orderService;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var orders = await _orderService.GetAllAsync();
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(orders, StatusCodes.Status200OK)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                ApiResponse<OrderResponse>.Failure(StatusCodes.Status404NotFound, "Order not exists.")
            );
        }

        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<OrderResponse>.SuccessResponse(order, StatusCodes.Status200OK)
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] OrderRequest request)
    {
        foreach (var item in request.OrderItems)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
            {
                return StatusCode(
                                StatusCodes.Status400BadRequest,
                                ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Requested Quantity not available.", ModelStateHelper.ToErrorResponse(ModelState))
                            );
            }
        }

        var id = await _orderService.CreateAsync(request, (int)HttpContext.Items["UserId"]);

        if (id == null)
        {
            return StatusCode(
                            StatusCodes.Status400BadRequest,
                            ApiResponse<string>.Failure(StatusCodes.Status400BadRequest, "Order not processed", ModelStateHelper.ToErrorResponse(ModelState))
                        );
        }


        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.SuccessResponse((int)id, StatusCodes.Status201Created)
        );
    }

}
