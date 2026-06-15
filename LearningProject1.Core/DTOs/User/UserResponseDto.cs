using LearningProject1.Core.DTOs.Order;

namespace LearningProject1.Core.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<OrderResponseDto> Orders { get; set; } = new List<OrderResponseDto>();
}
