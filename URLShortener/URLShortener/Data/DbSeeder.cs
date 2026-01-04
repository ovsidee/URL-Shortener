using Microsoft.AspNetCore.Identity;

namespace URLShortener.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        // Check if admin already exists
        var adminEmail = "admin@vitalii.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            // This handles the hashing automatically and correctly
            await userManager.CreateAsync(newAdmin, "Vitalii123!");
            
            // Assign role
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}