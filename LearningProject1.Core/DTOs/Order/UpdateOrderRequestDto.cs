using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningProject1.Core.DTOs.Order;

public class UpdateOrderRequestDto
{
    [Required]
    [DefaultValue("product")]
    [MaxLength(100)]
    public string Product { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
