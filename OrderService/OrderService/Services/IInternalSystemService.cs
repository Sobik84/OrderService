using OrderService.Models;

namespace OrderService.Services;

public interface IInternalSystemService
{
    Task Send(Order[] orders);
    
    Task<Order?> GetOrderByProductId(int productId);

    void CleanUp();
}