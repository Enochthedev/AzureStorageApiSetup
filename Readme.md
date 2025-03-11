# Azure Storage API Documentation

## **Azure Storage Configuration (`AzureStorageConfig`)**

This API now uses a **strongly-typed configuration model** called `AzureStorageConfig` to manage Azure Storage settings.

### **AzureStorageConfig.cs**

This class maps to settings stored in `appsettings.json`:

```csharp
public class AzureStorageConfig
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}
```

### **Configuration in `appsettings.json`**

Ensure your `appsettings.json` file contains the following:

```json
"AzureStorage": {
  "ConnectionString": "UseDevelopmentStorage=true",
  "ContainerName": "testcontainer"
}
```

### **Production Configuration**

Replace `"UseDevelopmentStorage=true"` with your **actual Azure Storage connection string** when deploying:

```json
"AzureStorage": {
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=<your-storage-account>;AccountKey=<your-account-key>;EndpointSuffix=core.windows.net",
  "ContainerName": "production-container"
}
```

---

## **Codebase Updates**

- **Dependency Injection**: The API now registers `AzureStorageConfig` in `Program.cs`:

  ```csharp
  builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorage"));
  ```

- **`BlobServiceClient` is properly registered**:

  ```csharp
  builder.Services.AddSingleton(_ =>
  {
      var config = builder.Configuration.GetSection("AzureStorage").Get<AzureStorageConfig>();
      return new BlobServiceClient(config.ConnectionString);
  });
  ```

---

## **Deployment Notes**

1. **Ensure `AzureStorageConfig` is properly set up.**
2. **Verify that `appsettings.json` contains correct storage settings.**
3. **Run the application:**

   ```sh
   dotnet run
   ```

This ensures the **Azure Storage API** is properly configured and runs without dependency injection errors.

---

These updates improve configuration management, making it easier to switch between **development and production** environments seamlessly. 🚀
