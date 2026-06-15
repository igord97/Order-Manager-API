using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningProject1.Core.Models;

[Table("Orders")]
public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Product { get; set; } = string.Empty;
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public decimal Total { get; set; }

    public int UserId { get; set; }
}