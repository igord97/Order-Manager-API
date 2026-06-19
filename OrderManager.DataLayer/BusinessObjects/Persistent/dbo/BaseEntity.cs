namespace OrderManager.DataLayer.BusinessObjects.Persistent.dbo;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}