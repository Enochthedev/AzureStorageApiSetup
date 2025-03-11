### **1. Upload a File**

To upload a file to Azure Blob Storage:

```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file, [FromServices] BlobServiceClient blobServiceClient)
{
    var containerClient = blobServiceClient.GetBlobContainerClient("testcontainer");
    var blobClient = containerClient.GetBlobClient(file.FileName);

    using (var stream = file.OpenReadStream())
    {
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    return Ok(new { FileName = file.FileName, Status = "Uploaded" });
}
```

#### **Request Format**

```http
POST /upload
Content-Type: multipart/form-data

Form Data:
- file: (binary file)
```

---

### **2. Download a File**

To retrieve a file from Azure Blob Storage:

```csharp
[HttpGet("download/{fileName}")]
public async Task<IActionResult> DownloadFile(string fileName, [FromServices] BlobServiceClient blobServiceClient)
{
    var containerClient = blobServiceClient.GetBlobContainerClient("testcontainer");
    var blobClient = containerClient.GetBlobClient(fileName);

    if (await blobClient.ExistsAsync())
    {
        var stream = await blobClient.OpenReadAsync();
        return File(stream, "application/octet-stream", fileName);
    }

    return NotFound("File not found");
}
```

#### **Request Format**

```http
GET /download/{fileName}
```

#### **Response**

- **200 OK**: Returns the requested file.
- **404 Not Found**: If the file does not exist.

---

### **3. Delete a File**

To delete a file from Azure Blob Storage:

```csharp
[HttpDelete("delete/{fileName}")]
public async Task<IActionResult> DeleteFile(string fileName, [FromServices] BlobServiceClient blobServiceClient)
{
    var containerClient = blobServiceClient.GetBlobContainerClient("testcontainer");
    var blobClient = containerClient.GetBlobClient(fileName);

    if (await blobClient.ExistsAsync())
    {
        await blobClient.DeleteAsync();
        return Ok(new { FileName = fileName, Status = "Deleted" });
    }

    return NotFound("File not found");
}
```

#### **Request Format**

```http
DELETE /delete/{fileName}
```

#### **Response**

- **200 OK**: File successfully deleted.
- **404 Not Found**: If the file does not exist.

---

### **4. List All Files in a Container**

To list all files inside the configured Azure Storage container:

```csharp
[HttpGet("list")]
public async Task<IActionResult> ListFiles([FromServices] BlobServiceClient blobServiceClient)
{
    var containerClient = blobServiceClient.GetBlobContainerClient("testcontainer");
    var blobs = new List<string>();

    await foreach (var blob in containerClient.GetBlobsAsync())
    {
        blobs.Add(blob.Name);
    }

    return Ok(blobs);
}
```

#### **Request Format**

```http
GET /list
```

#### **Response**

- **200 OK**: Returns a list of file names in the storage container.

---

These additions clarify how to **make API requests** and the **expected responses**. 🚀
