using System.Text;
using System.Text.Json;
using static System.Text.Json.JsonDocument;

namespace OrderServiceTests;

public class Tests
{
    [Test]
    // This test is very simple, but it covers the whole use case of our web service
    public async Task Test1()
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5154/api/orders/"),
        };
        
        // clean data in internal system
        await CleanUp(httpClient);
        
        // receive some orders and do the process (aggregate and send orders to internal system)
        await PostTestData(httpClient);
        
        // we are waiting here for the results in internal system (not nice, but simple for now)
        await Task.Delay(40 * 1000);
        
        // check the results
        
        var order1 = await GetOrder(httpClient, 1);
        Assert.That(order1.RootElement.GetProperty("quantity").GetInt32(), Is.EqualTo(106));
        
        var order2 = await GetOrder(httpClient, 2);
        Assert.That(order2.RootElement.GetProperty("quantity").GetInt32(), Is.EqualTo(46));
    }
    
    static async Task PostTestData(HttpClient httpClient)
    {
        var arr = new[]
        {
            new
            {
                productId = 1,
                quantity = 102,
            },
            new
            {
                productId = 1,
                quantity = 4,
            },
            new
            {
                productId = 2,
                quantity = 46,
            }
        };
        
        using StringContent jsonContent = new(JsonSerializer.Serialize(arr), Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await httpClient.PostAsync("receive", jsonContent);
        response.EnsureSuccessStatusCode();
    }
    
    static async Task CleanUp(HttpClient httpClient)
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync("cleanup");
        response.EnsureSuccessStatusCode();
    }
    
    static async Task<JsonDocument> GetOrder(HttpClient httpClient, int productId)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("order?productId=" + productId);
        response.EnsureSuccessStatusCode();

        return Parse(await response.Content.ReadAsStringAsync());
    }
}