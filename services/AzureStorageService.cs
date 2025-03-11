using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class AzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureStorageService> _logger;

    public AzureStorageService(IConfiguration configuration, BlobServiceClient blobServiceClient, ILogger<AzureStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["AzureStorage:ContainerName"] ?? throw new ArgumentNullException("AzureStorage:ContainerName");
        _logger = logger;
    }

public async Task<(string fileUrl, string relativePath)> UploadFileAsync(Stream fileStream, string tenantId, string company, string category, string fileId, string contentType)
{
    try
    {
        _logger.LogInformation("Starting file upload. Tenant: {TenantId}, Company: {Company}, Category: {Category}, FileId: {FileId}", tenantId, company, category, fileId);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        _logger.LogInformation("Blob container {ContainerName} checked/created.", _containerName);

        string blobPath = $"{tenantId}/{company}/{category}/{fileId}";
        _logger.LogInformation("Uploading file to Blob Path: {BlobPath}", blobPath);

        var blobClient = blobContainerClient.GetBlobClient(blobPath);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }).ConfigureAwait(false);

        string fileUrl = blobClient.Uri.ToString();
        string relativePath = $"{category}/{fileId}";

        _logger.LogInformation("File uploaded successfully: {FileUrl}, Relative Path: {RelativePath}", fileUrl, relativePath);

        return (fileUrl, relativePath);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error uploading file.");
        throw;
    }
}
    public async Task<Stream> DownloadFileAsync(string blobName)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync().ConfigureAwait(false))
            {
                _logger.LogWarning("File {BlobName} not found.", blobName);
                throw new FileNotFoundException("File not found.");
            }

            var download = await blobClient.DownloadContentAsync().ConfigureAwait(false);
            _logger.LogInformation("File downloaded successfully: {BlobName}", blobName);

            return download.Value.Content.ToStream();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file.");
            throw;
        }
    }

    public async Task DeleteFileAsync(string blobName)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            bool deleted = await blobClient.DeleteIfExistsAsync().ConfigureAwait(false);
            if (deleted)
            {
                _logger.LogInformation("File deleted successfully: {BlobName}", blobName);
            }
            else
            {
                _logger.LogWarning("File {BlobName} not found for deletion.", blobName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file.");
            throw;
        }
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("your-container-name");
        var blobs = new List<string>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            blobs.Add(blob.Name);
        }

        return blobs;
    }   
}