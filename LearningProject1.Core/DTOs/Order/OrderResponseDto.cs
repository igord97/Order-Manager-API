namespace LearningProject1.Core.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; } 
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public int UserId { get; set; }
}
