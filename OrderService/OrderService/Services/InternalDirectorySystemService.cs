using OrderService.Models;
using System.Text.Json;

namespace OrderService.Services;

// This class represents internal system and it is represented as directory in filesystem
public class InternalDirectorySystemService : IInternalSystemService
{
    // we can take this path from some config
    private readonly string _directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "InternalSystem");

    public async Task Send(Order[] orders)
    {
        // create directory when it does not exist
        Directory.CreateDirectory(_directoryPath);
        
        // save given orders to internal system
        foreach (var order in orders)
        {
            var filename = Path.Combine(_directoryPath, order.ProductId.ToString());

            Order orderToSave; 

            if (File.Exists(filename))
            {
                var json = await File.ReadAllTextAsync(filename);
                var exiOrder = JsonSerializer.Deserialize<Order?>(json)!;
                
                orderToSave = order with { Quantity = order.Quantity + exiOrder.Quantity };
            }
            else 
            {
                orderToSave = new Order
                {
                    ProductId = order.ProductId,
                    Quantity = order.Quantity
                };
            }
            
            var jsonToSave = JsonSerializer.Serialize(orderToSave);
        
            await File.WriteAllTextAsync(filename, jsonToSave);
        }
    }
    
    public async Task<Order?> GetOrderByProductId(int productId)
    {
        var filename = Path.Combine(_directoryPath, productId.ToString());

        if (!File.Exists(filename)) return null;
        
        var json = await File.ReadAllTextAsync(filename);
        
        return JsonSerializer.Deserialize<Order>(json);
    }
    
    public void CleanUp() => Directory.GetFiles(_directoryPath).ToList().ForEach(File.Delete);
}