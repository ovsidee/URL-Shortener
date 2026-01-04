using Microsoft.AspNetCore.Identity;

namespace URLShortener.Data;

public static class DbSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // check whether users exist
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // create role if does`nt exist
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // admin
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
            
            await userManager.CreateAsync(newAdmin, "Vitalii123!");
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }

        // default user
        var userEmail = "user@vitalii.com";
        var normalUser = await userManager.FindByEmailAsync(userEmail);

        if (normalUser == null)
        {
            var newUser = new IdentityUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(newUser, "User123!");
            await userManager.AddToRoleAsync(newUser, "User");
        }
    }
}