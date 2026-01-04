using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Services;

namespace URLShortener;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // SQLite
        var connectionString = builder.Configuration.GetConnectionString("connectionString");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
        
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        }).AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();
        
        builder.Services.AddControllersWithViews();

        //----DI
        builder.Services.AddScoped<IUrlApiService, UrlApiService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAboutService, AboutService>(); 
        
        // CORS Service
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularDev",
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Angular port
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // Auth Cookies
                });
        });
        
        var app = builder.Build();
        
        // seed data
        using (var scope = app.Services.CreateScope())
        {
            try 
            {
                await DbSeeder.SeedAdminAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding Error: {ex.Message}");
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        
        app.UseRouting();
        
        app.UseCors("AllowAngularDev"); // enable the policy
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        await app.RunAsync();
    }
}