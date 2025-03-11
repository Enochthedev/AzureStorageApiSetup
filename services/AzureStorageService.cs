using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

public class AzureStorageService(IConfiguration configuration)
{
#pragma warning disable CS8601 // Possible null reference assignment.
    private readonly string _connectionString = configuration["AzureStorage:ConnectionString"];
#pragma warning restore CS8601 // Possible null reference assignment.
    private readonly string _containerName = configuration["AzureStorage:ContainerName"] ?? throw new ArgumentNullException("AzureStorage:ContainerName");

    public async Task<string> UploadFileAsync(Stream fileStream, string tenantId, string company, string category, string fileId, string contentType)
    {
    var blobServiceClient = new BlobServiceClient(_connectionString);
    var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

    // Ensure the container exists (without public access)
    await blobContainerClient.CreateIfNotExistsAsync();

    // Construct the full path in "folder" format
    string blobPath = $"{tenantId}/{company}/{category}/{fileId}";
    var blobClient = blobContainerClient.GetBlobClient(blobPath);

    // Check if the file already exists before uploading
    if (await blobClient.ExistsAsync())
    {
        throw new Exception("File already exists!");
    }

    // Upload the file
    await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

    return blobPath; // Return the relative path instead of the full URL
    }   
    public async Task<Stream> DownloadFileAsync(string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        var download = await blobClient.DownloadContentAsync();
        return download.Value.Content.ToStream();
    }

    public async Task DeleteFileAsync(string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}