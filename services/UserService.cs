using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AzureStorageApi.Services
{
    public class UserService
    {
        private readonly AzureStorageService _azureStorageService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AzureStorageService azureStorageService, ApplicationDbContext context, ILogger<UserService> logger)
        {
            _azureStorageService = azureStorageService;
            _context = context;
            _logger = logger;
        }

        public async Task<string> UploadUserProfilePictureAsync(Guid userId, Stream fileStream, string fileName)
        {
            string fileId = $"{userId}/{fileName}";
            _logger.LogInformation("Uploading user profile picture for User: {UserId}", userId);

            try
            {
                string company = "YourCompany"; // Replace with actual company value
                var (fileUrl, relativePath) = await _azureStorageService.UploadFileAsync(fileStream, fileId, company, string.Empty, string.Empty, string.Empty);
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.ProfilePictureUrl = fileUrl;
                    await _context.SaveChangesAsync();
                }

                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading user profile picture.");
                throw;
            }
        }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new Exception($"User with ID {id} not found.");
        }
        return user;
    }    
    

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    
    public async Task<User> CreateUserAsync(User user, Stream? profilePictureStream = null, string? profilePictureFileName = null)
    {
        string? uploadedFileUrl = null;
        string? fileId = null;

        if (profilePictureStream != null && !string.IsNullOrEmpty(profilePictureFileName))
        {
            try
            {
                string company = "YourCompany"; // Replace with actual company value
                fileId = $"{user.Id}/{profilePictureFileName}";
                var (fileUrl, relativePath) = await _azureStorageService.UploadFileAsync(profilePictureStream, fileId, company, string.Empty, string.Empty, string.Empty);
                uploadedFileUrl = fileUrl;
                user.ProfilePictureUrl = uploadedFileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture during user creation.");
                throw;
            }
        }

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user. Reverting uploaded profile picture if applicable.");

            if (!string.IsNullOrEmpty(uploadedFileUrl) && !string.IsNullOrEmpty(fileId))
            {
                try
                {
                    await _azureStorageService.DeleteFileAsync(fileId);
                    _logger.LogInformation("Reverted uploaded profile picture: {FileId}", fileId);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Error reverting uploaded profile picture.");
                }
            }

            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }


}}