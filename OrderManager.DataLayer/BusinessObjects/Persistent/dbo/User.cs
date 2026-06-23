using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

[Table("Users", Schema = "dbo")]
public class User : BaseEntity
{
    [Required]
    [MaxLength(50)]
    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("Role")]
    public string Role { get; set; } = "User";

    public List<Order> Orders { get; set; } = new();
}