using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

public abstract class BaseEntity
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}