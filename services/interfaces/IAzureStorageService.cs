public interface IAzureStorageService
{
    Task<FileUploadResponse> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string blobName);
    Task DeleteFileAsync(string blobName);
    Task<List<string>> ListFilesAsync();
}

public class FileUploadResponse
{
}