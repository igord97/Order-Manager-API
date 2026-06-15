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

    public int UserId { get; set; }

    public User User { get; set; }
}