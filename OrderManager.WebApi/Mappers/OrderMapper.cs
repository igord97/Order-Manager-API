using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;
using OrderManager.WebApi.Models.Order;

namespace OrderManager.WebApi.Mappers;

public static class OrderMapper
{
    public static Order ToEntity(CreateOrderRequest request)
    {
        return new Order
        {
            UserId = request.UserId,
            Product = request.Product,
            Quantity = request.Quantity,
            Price = request.Price,
            Total = request.Quantity * request.Price
        };
    }

    public static OrderResponseDto ToResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            Product = order.Product,
            Quantity = order.Quantity,
            Price = order.Price,
            Total = order.Total,
            UserId = order.UserId
        };
    }
}