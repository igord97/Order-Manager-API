using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.Models;

namespace LearningProject1.Core.Mappers;

public class OrderMapper
{
    public static Order ToEntity(OrderRequestDto orderRequestDto)
    {
        return new Order
        {
            Product = orderRequestDto.Product,
            UserId = orderRequestDto.UserId
        };
    }

    public static OrderResponseDto ToResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            Product = order.Product,
            UserId = order.UserId
        };
    }
}
