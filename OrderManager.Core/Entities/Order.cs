using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace OrderManager.Core.Entities;

[Table("Orders", Schema="dbo")]
public class Order : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Product { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}