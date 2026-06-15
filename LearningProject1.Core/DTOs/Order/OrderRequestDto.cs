namespace LearningProject1.Core.DTOs.Order;

public class OrderRequestDto
{
    public string Product { get; set; } = string.Empty;
    public int UserId { get; set; }
}
