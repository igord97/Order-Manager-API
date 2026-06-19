using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderManager.WebApi.Models.Order;

public class OrderRequestDto
{
    [Required]
    [DefaultValue("product")]
    [MaxLength(100)]
    public string Product { get; set; } = string.Empty;

    [DefaultValue(9.99)]
    public decimal Price { get; set; } = 9.99m;

    [DefaultValue(1)]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}
