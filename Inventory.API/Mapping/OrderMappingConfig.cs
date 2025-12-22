using Inventory.Models.Entities;
using Inventory.Models.Order;
using Mapster;

namespace Inventory.API.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderResponse>()
            .Map(d => d.UserName,
                s => s.User != null && s.User.UserProfile != null
                    ? s.User.UserProfile.FirstName + " " +
                      s.User.UserProfile.LastName
                    : string.Empty);

        config.NewConfig<OrderItem, OrderItemResponse>()
            .Map(d => d.ProductName,
                s => s.Product != null ? s.Product.Name : string.Empty);
    }
}
