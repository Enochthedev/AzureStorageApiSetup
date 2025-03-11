using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly AzureStorageService _storageService;
    private readonly ILogger<StorageController> _logger;

    public StorageController(AzureStorageService storageService)
    {
        _storageService = storageService;
        // Add logging
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<StorageController>();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromQuery] string tenantId, [FromQuery] string company, [FromQuery] string category, [FromForm] string fileId, [FromForm] IFormFile file)
    {
        try
        {
            // Open the file stream
            using var stream = file.OpenReadStream();
            
            
            _logger.LogInformation($"Uploading file {file.FileName} for {tenantId}/{company}/{category}/{fileId}");
            // Upload the file
            var fileUrl = await _storageService.UploadFileAsync(
                stream,
                tenantId,
                company,
                category,
                fileId,
                file.ContentType
            );

            _logger.LogInformation($"File uploaded successfully: {fileUrl}");

            // Return the relative file location
            return Ok(new { Location = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during file upload. Tenant: {TenantId}, Company: {Company}, Category: {Category}, FileId: {FileId}", tenantId, company, category, fileId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("download/{blobName}")]
    public async Task<IActionResult> DownloadFile(string blobName)
    {
        try
        {
            var stream = await _storageService.DownloadFileAsync(blobName);
            return File(stream, "application/octet-stream", blobName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpDelete("delete/{blobName}")]
    public async Task<IActionResult> DeleteFile(string blobName)
    {
        try
        {
            await _storageService.DeleteFileAsync(blobName);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
}