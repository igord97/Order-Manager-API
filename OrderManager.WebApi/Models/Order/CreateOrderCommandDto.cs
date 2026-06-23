namespace OrderManager.WebApi.Models.Order;

public class CreateOrderCommandDto
{
    public int UserId { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}