# Azure Storage API Documentation

## Overview

The **Azure Storage API** is designed to interact with **Azure Blob Storage**, enabling users to store, retrieve, and manage files efficiently. This API follows **Azure Blob Storage naming conventions** and ensures compliance with best practices.

---

## Features

- **Blob Storage Management**: Upload, download, delete, and list files in Azure Blob Storage.
- **Tenant-Specific Storage Structure**: Organizes files by `{tenant-id}/{category}/{uuid-or-hash}.{ext}` for efficient retrieval.
- **Security & Authentication**: Uses **Azure Storage Account Keys** or **Azure Active Directory (AAD) tokens** for secure access.
- **Scalability**: Supports large-scale storage operations with **containerized storage**.

---

## Azure Blob Naming Conventions

### Key Rules

#### Allowed Characters

- Alphanumerics (`a-z`, `A-Z`, `0-9`)
- Hyphen (`-`), underscore (`_`), period (`.`), and forward slash (`/`) for virtual directories
- Special characters should be **URL-encoded** (e.g., space → `%20`)

#### Restrictions

- Case-sensitive (recommended: use **lowercase consistently**)
- Maximum length: **1024 characters**
- Cannot end with a forward slash (`/`) except for root directories
- Avoid **reserved URL characters**: `& $ @ = ? ; : + , * ' %`

### Tenant-Specific Structure

Each file follows a **standardized path format**:

```
{tenant-id}/{category}/{uuid-or-hash}.{ext}
```

### Example Paths

- `contoso/documents/3F2504E0-4F89-11D3-9A0C-0305E82C3301.pdf`
- `acme/images/profile/859b39a7-075c-4a5e-8d20-cc078d3f5613.jpg`

---

## API Endpoints

### 1. **Upload File**

**Endpoint:**

```
POST /upload
```

**Description:**
Uploads a file to Azure Blob Storage under the specified tenant.

**Request Body:**

```json
{
  "tenantId": "contoso",
  "category": "documents",
  "file": "<binary-data>"
}
```

**Response:**

```json
{
  "message": "File uploaded successfully",
  "fileUrl": "https://yourstorage.blob.core.windows.net/contoso/documents/3F2504E0-4F89-11D3-9A0C-0305E82C3301.pdf"
}
```

---

### 2. **Download File**

**Endpoint:**

```
GET /download/{tenantId}/{category}/{fileId}
```

**Description:**
Retrieves a file from Azure Blob Storage.

**Response:**
Returns the requested file as **binary data**.

---

### 3. **List Files**

**Endpoint:**

```
GET /list/{tenantId}/{category}
```

**Description:**
Lists all files within a specified **tenant category**.

**Response:**

```json
{
  "files": [
    {
      "fileId": "3F2504E0-4F89-11D3-9A0C-0305E82C3301.pdf",
      "fileUrl": "https://yourstorage.blob.core.windows.net/contoso/documents/3F2504E0-4F89-11D3-9A0C-0305E82C3301.pdf"
    },
    {
      "fileId": "859b39a7-075c-4a5e-8d20-cc078d3f5613.jpg",
      "fileUrl": "https://yourstorage.blob.core.windows.net/acme/images/profile/859b39a7-075c-4a5e-8d20-cc078d3f5613.jpg"
    }
  ]
}
```

---

### 4. **Delete File**

**Endpoint:**

```
DELETE /delete/{tenantId}/{category}/{fileId}
```

**Description:**
Deletes a file from Azure Blob Storage.

**Response:**

```json
{
  "message": "File deleted successfully"
}
```

---

## Authentication

This API supports **two authentication methods**:

1. **Azure Storage Account Key** (for direct storage access)
2. **Azure Active Directory (AAD) Bearer Token** (recommended for enterprise security)

To authenticate, include a **Bearer Token** in the request header:

```
Authorization: Bearer <your_token>
```

---

## Environment Variables

Configure the following environment variables before running the API:

```plaintext
AZURE_STORAGE_ACCOUNT_NAME=<your_storage_account>
AZURE_STORAGE_ACCOUNT_KEY=<your_storage_key>
AZURE_STORAGE_CONNECTION_STRING=<your_connection_string>
```

---

## Deployment

To deploy this API, follow these steps:

1. Clone the repository:

   ```sh
   git clone https://github.com/your-repo/AzureStorageApi.git
   cd AzureStorageApi
   ```

2. Install dependencies:

   ```sh
   npm install
   ```

3. Start the API:

   ```sh
   npm start
   ```

---

## Conclusion

This API enables **secure and efficient** file management with **Azure Blob Storage** while enforcing naming conventions and tenant-based structuring. Future enhancements may include **automatic lifecycle management** and **policy-based access control**.

---

## **Azure Storage Connection and Configuration**

### **Understanding the Azure Connection String**

In your **appsettings.json**, the following configuration is used:

```json
"AzureStorage": {
  "ConnectionString": "UseDevelopmentStorage=true",
  "ContainerName": "testcontainer"
}
```

This indicates that the API is **using Azure Storage Emulator** (or **Azurite**) for local development. The `UseDevelopmentStorage=true` setting allows developers to test **Azure Blob Storage functionality** on their local machines without needing an actual Azure account.

### **What Does a Production Connection String Look Like?**

In a production environment, you must replace `"UseDevelopmentStorage=true"` with an actual **Azure Storage connection string**, which typically follows this format:

```json
"AzureStorage": {
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=<your-storage-account>;AccountKey=<your-account-key>;EndpointSuffix=core.windows.net",
  "ContainerName": "production-container"
}
```

#### **Breaking Down the Connection String:**

- **`DefaultEndpointsProtocol=https;`** → Ensures secure HTTPS communication.
- **`AccountName=<your-storage-account>;`** → Specifies the Azure Storage account name.
- **`AccountKey=<your-account-key>;`** → The secret key used for authentication.
- **`EndpointSuffix=core.windows.net`** → Defines the Azure Blob Storage endpoint.

---

## **Codebase Explanation**

This project provides an API for managing files in **Azure Blob Storage**, including **upload, download, and delete** operations.

### **Key Components:**

1. **`StorageController.cs`**:
   - Handles HTTP requests for file operations.
   - Uses dependency injection to call `AzureStorageService`.
   - Implements structured logging and error handling.

2. **`AzureStorageService.cs`**:
   - Communicates with Azure Blob Storage.
   - Manages uploads, downloads, and deletions efficiently.
   - Uses `BlobServiceClient` to connect to Azure.

3. **Blob Connection Configuration**:
   - The API retrieves the Azure Storage connection string from **environment variables** or `appsettings.json`.
   - In **development**, it uses `"UseDevelopmentStorage=true"`.
   - In **production**, it requires a **valid Azure connection string**.

---

These updates ensure that developers understand the difference between **local and production configurations** and how to properly secure their storage connections. 🚀
