using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IStorageService, StorageDirectoryService>();
builder.Services.AddSingleton<IProcessingService, ProcessingService>();
builder.Services.AddSingleton<IInternalSystemService, InternalDirectorySystemService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var processingService = app.Services.GetService<IProcessingService>();
processingService?.Run();

app.Run();