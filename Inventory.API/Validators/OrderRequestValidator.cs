using FluentValidation;
using Inventory.Models.Order;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Inventory.Common.Enums;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator(IProductService productService)
    {
        RuleFor(x => x.Status)
            .GreaterThanOrEqualTo(1).LessThanOrEqualTo(4)
            .WithMessage("Invalid order status.");

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemRequestValidator());
    }
}
