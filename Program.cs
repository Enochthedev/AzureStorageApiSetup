using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add configuration for Azure Storage
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorage"));

// Register BlobServiceClient (REQUIRED)
builder.Services.AddSingleton(_ =>
{
    var connectionString = builder.Configuration.GetValue<string>("AzureStorage:ConnectionString");
    return new BlobServiceClient(connectionString);
});

// Register AzureStorageService
builder.Services.AddScoped<AzureStorageService>();

var app = builder.Build();

app.Run();