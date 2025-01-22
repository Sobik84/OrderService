using OrderService.Models;

namespace OrderService.Services;

public interface IStorageService
{
    Task Save(Order[] orders);

    string[] GetOrdersIds();

    Task<Order[]> GetOrdersById(string id);
    
    void RemoveOrdersById(string id);
}