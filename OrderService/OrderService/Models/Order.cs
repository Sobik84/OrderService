namespace OrderService.Models;

public record class Order
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}