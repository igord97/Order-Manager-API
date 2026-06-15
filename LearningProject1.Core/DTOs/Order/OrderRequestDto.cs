namespace LearningProject1.Core.DTOs.Order;

public class OrderRequestDto
{
    public int UserId { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; } = 0m;
}
