using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly AzureStorageService _storageService;
    private readonly ILogger<StorageController> _logger;

    public StorageController(AzureStorageService storageService, ILogger<StorageController> logger)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(
        [FromQuery] string tenantId, 
        [FromQuery] string company, 
        [FromQuery] string category, 
        [FromForm] string fileId, 
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Upload request received with an empty file.");
            return BadRequest(new { error = "File is required." });
        }

        try
        {
            using var stream = file.OpenReadStream();

            _logger.LogInformation("Uploading file: {FileName} for Tenant: {TenantId}, Company: {Company}, Category: {Category}, FileId: {FileId}", 
                file.FileName, tenantId, company, category, fileId);

            var (fileUrl, relativePath) = await _storageService.UploadFileAsync(
            stream, tenantId, company, category, fileId, file.ContentType);


            _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);

            return Ok(new { location = fileUrl, relativePath = relativePath });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName} for Tenant: {TenantId}, Company: {Company}, Category: {Category}, FileId: {FileId}", 
                file.FileName, tenantId, company, category, fileId);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [HttpGet("download/{blobName}")]
    public async Task<IActionResult> DownloadFile(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            _logger.LogWarning("Download request received without a valid blob name.");
            return BadRequest(new { error = "Blob name is required." });
        }

        try
        {
            _logger.LogInformation("Downloading file: {BlobName}", blobName);
            var stream = await _storageService.DownloadFileAsync(blobName);

            return File(stream, "application/octet-stream", blobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {BlobName}", blobName);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [HttpDelete("delete/{blobName}")]
    public async Task<IActionResult> DeleteFile(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            _logger.LogWarning("Delete request received without a valid blob name.");
            return BadRequest(new { error = "Blob name is required." });
        }

        try
        {
            _logger.LogInformation("Deleting file: {BlobName}", blobName);
            await _storageService.DeleteFileAsync(blobName);

            return Ok(new { message = "File deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {BlobName}", blobName);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    [HttpGet("list")]
public async Task<IActionResult> ListFiles()
{
    try
    {
        _logger.LogInformation("Listing all files in storage.");
        var files = await _storageService.ListFilesAsync();

        return Ok(files);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving file list.");
        return StatusCode(500, new { error = "Internal server error", details = ex.Message });
    }
}
}
