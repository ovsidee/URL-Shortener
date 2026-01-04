using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URLShortener.Models;

namespace URLShortener.Data;

public class AppDbContext : IdentityDbContext
{
    public DbSet<ShortUrl> ShortUrls { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {
    }

    protected AppDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // 1. Enforce unique URLs
        builder.Entity<ShortUrl>()
            .HasIndex(u => u.OriginalURL)
            .IsUnique();
        
        // 2. We ONLY seed the Roles here because they are static
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "8af10569-b018-4fe7-a380-7d6a14c70b74", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "User", NormalizedName = "USER" }
        );
    }
}