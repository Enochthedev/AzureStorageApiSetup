using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using AzureStorageApi.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // ✅ Register controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorage"));


// Connect to SQL Server (MSSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserService>();
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
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization(); // Optional, if needed for authentication
app.MapControllers(); // ✅ This registers the API endpoints

app.Run();