var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // ✅ Ensure controllers are registered
builder.Services.AddScoped<AzureStorageService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // ✅ Ensure controller endpoints are mapped


app.Run();