namespace LearningProject1.Core.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public int UserId { get; set; }
}
