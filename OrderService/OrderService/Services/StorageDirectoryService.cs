using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services;

// This storage is implemented using directory in filesystem
public class StorageDirectoryService : IStorageService
{
    // we can take this path from some config
    private readonly string _directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
    
    private const string JsonFileExtension = ".json";
    
    private const string SavFileExtension = ".sav";

    public async Task Save(Order[] orders)
    {
        // create directory when it does not exist
        Directory.CreateDirectory(_directoryPath);

        // create guid for the given orders and serialize them to JSON
        var id = Guid.NewGuid().ToString();
        var json = JsonSerializer.Serialize(orders);
        
        // write JSON
        await File.WriteAllTextAsync(GetJsonFilePath(id), json);
        
        // create another file which helps synchronize this thread with consumer (ProcessingService)
        await File.WriteAllTextAsync(GetSavFilePath(id), string.Empty);
    }
    
    public string[] GetOrdersIds()
    {
        var files = Directory.GetFiles(_directoryPath, "*" + SavFileExtension);
        
        return files.Select(x => Path.GetFileNameWithoutExtension(x).ToString()).ToArray();
    }
    
    public async Task<Order[]> GetOrdersById(string id)
    {
        var json = await File.ReadAllTextAsync(GetJsonFilePath(id));
        
        return JsonSerializer.Deserialize<Order[]?>(json)!.ToArray();
    }
    
    public void RemoveOrdersById(string id)
    {
        File.Delete(GetSavFilePath(id));
        File.Delete(GetJsonFilePath(id));
    }
    
    private string GetJsonFilePath(string id) => Path.Combine(_directoryPath, id + JsonFileExtension);
    
    private string GetSavFilePath(string id) => Path.Combine(_directoryPath, id + SavFileExtension);
}