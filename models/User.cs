namespace AzureStorageApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}