using LearningProject1.Core.DTOs.Order;
using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Models;

namespace LearningProject1.Core.Mappers;

public class OrderMapper
{
    public static Order ToEntity(OrderRequestDto orderRequestDto)
    {
        return new Order
        {
            Product = orderRequestDto.Product,
            Quantity = orderRequestDto.Quantity,
            Price = orderRequestDto.Price,
            UserId = orderRequestDto.UserId
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

    public static UpdateUserResponseDto ToUpdateResponseDto(User user)
    {
        return new UpdateUserResponseDto
        {
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email
        };
    }
}
