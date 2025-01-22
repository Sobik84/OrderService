using OrderService.Services;

namespace OrderService.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

[ApiController]
[Route("api/Orders")]
public class OrdersController(IStorageService storageService, IInternalSystemService internalSystem) : ControllerBase
{
    // This method quickly saves received orders to storage
    [HttpPost("receive")]
    public async Task Receive([FromBody] Order[] orders)
    {
        if (orders.Length == 0) return;
        
        await storageService.Save(orders);
    }
    
    // This method is here for automatic test only
    [HttpGet("order")]
    public async Task<Order?> GetOrderFromInternalSystem(int productId)
    {
        return await internalSystem.GetOrderByProductId(productId);
    }
    
    // This method is here for automatic test only
    [HttpDelete("cleanup")]
    public void CleanUpInternalSystem()
    {
        internalSystem.CleanUp();
    }
}
