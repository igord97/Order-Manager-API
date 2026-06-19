using OrderManager.DataLayer.BusinessObjects.Persistent.dbo;
using OrderManager.WebApi.Models.User;

namespace OrderManager.WebApi.Mappers;

public static class UserMapper
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