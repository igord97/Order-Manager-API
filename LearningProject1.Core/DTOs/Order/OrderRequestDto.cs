using System.ComponentModel;

namespace LearningProject1.Core.DTOs.Order;

public class OrderRequestDto
{
    [DefaultValue(1)]
    public int UserId { get; set; } = 1;
    [DefaultValue("example")]
    public string Product { get; set; } = string.Empty;
    [DefaultValue(1)]
    public int Quantity { get; set; } = 1;
    [DefaultValue(0)]
    public decimal Price { get; set; } = 0m;
}
