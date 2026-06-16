using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningProject1.Core.DTOs.Order;

public class OrderRequestDto
{
    [DefaultValue(1)]
    [Required]
    public int UserId { get; set; } = 1;
    
    [Required]
    [MaxLength(100)]
    public string Product { get; set; } = string.Empty;
    
    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [DefaultValue(9.99)]
    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal Price { get; set; } = 9.99m;
}
