using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using AzureStorageApi.Models; // Replace 'YourNamespace.Models' with the actual namespace where the User class is defined
public class User
{
    internal string ProfilePictureUrl = string.Empty;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=AzureStorageDb;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True");
        }
    }
}