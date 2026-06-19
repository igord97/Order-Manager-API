using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

[Table("Orders", Schema = "dbo")]
public class Order : BaseEntity
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Product")]
    public string Product { get; set; } = string.Empty;

    [Required]
    [Column("Quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("Price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column("Total", TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Required]
    [Column("UserId")]
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}