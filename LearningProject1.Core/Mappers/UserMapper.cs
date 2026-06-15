using LearningProject1.Core.DTOs.User;
using LearningProject1.Core.Models;

namespace LearningProject1.Core.Mappers;

public class UserMapper
{
    public static User ToEntity(UserRequestDto userRequestDto)
    {
        return new User
        {
            Name = userRequestDto.Name,
            Email = userRequestDto.Email
        };
    }

    public static UserResponseDto ToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Orders = user.Orders
                .Select(OrderMapper.ToResponseDto)
                .OrderBy(order => order.Id)
                .ToList()
        };
    }
}
