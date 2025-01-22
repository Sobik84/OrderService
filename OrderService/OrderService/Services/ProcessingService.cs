using OrderService.Models;

namespace OrderService.Services;

// This service consumes orders from storage, aggregate them and send to internal system. 
// Processed orders are also removed from the storage. 
public class ProcessingService(IStorageService storage, IInternalSystemService internalSystem) : IProcessingService
{
    public void Run()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                // we do the process every 20 seconds from the last ran
                await Task.Delay(20 * 1000);
                
                // here we can do better processing of orders from storage in batches
                var ordersIds = storage.GetOrdersIds();
                if (!ordersIds.Any()) continue;
                
                var aggregatedOrders = await AggregateOrders(ordersIds);
                
                var ordersToSend = aggregatedOrders.Select(x => new Order() { ProductId = x.Key, Quantity = x.Value }).ToArray();
                await internalSystem.Send(ordersToSend);
                
                RemoveOrders(ordersIds);
            }
        });
    }
    
    private async Task<Dictionary<int, int>> AggregateOrders(string[] ordersIds)
    {
        var result = new Dictionary<int, int>();
        foreach (var id in ordersIds)
        {
            foreach (var order in await storage.GetOrdersById(id))
            {
                result.TryGetValue(order.ProductId, out var quantity);
                result[order.ProductId] = order.Quantity + quantity;
            }
        }
        
        return result;
    }
    
    private void RemoveOrders(string[] ordersIds)
    {
        foreach (var id in ordersIds)
        {
            storage.RemoveOrdersById(id);   
        }
    }
}