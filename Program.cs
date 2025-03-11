using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // ✅ Register controllers
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorage"));

// Register BlobServiceClient
builder.Services.AddSingleton(_ =>
{
    var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
    return new BlobServiceClient(connectionString);
});

// Register AzureStorageService
builder.Services.AddScoped<AzureStorageService>();

var app = builder.Build();

// Enable routing and map controllers
app.UseRouting();
app.UseAuthorization(); // Optional, if needed for authentication
app.MapControllers(); // ✅ This registers the API endpoints

app.Run();