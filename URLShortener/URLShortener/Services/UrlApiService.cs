using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Models;

namespace URLShortener.Services;

public class UrlApiService : IUrlApiService
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UrlApiService(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ShortUrl>> GetAllUrlsAsync(CancellationToken cancellationToken)
    {
        return await _context
            .ShortUrls
            .ToListAsync(cancellationToken);
    }

    public async Task<ShortUrl?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context
            .ShortUrls
            .FindAsync([id], cancellationToken);
    }

    public async Task<ShortUrl?> GetByPathAsync(string shortCode, CancellationToken cancellationToken)
    {
        return await _context
            .ShortUrls
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode, cancellationToken);
    }

    public async Task<ShortUrl> AddUrlAsync(string originalUrl, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken)
    {
        // Check if URL already exists [Source: 18]
        var existing = await _context.ShortUrls
            .FirstOrDefaultAsync(u => u.OriginalURL == originalUrl, cancellationToken);

        if (existing != null)
        {
            throw new InvalidOperationException("This URL already exists.");
        }

        var user = await _userManager.GetUserAsync(userPrincipal);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        
        var shortUrl = new ShortUrl
        {
            OriginalURL = originalUrl,
            CreatedBy = user.UserName!,
            CreatedDate = DateTime.UtcNow,
            ShortCode = GenerateShortCode()
        };

        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync(cancellationToken);

        return shortUrl;
    }

    public async Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken)
    {
        var url = await _context.ShortUrls.FindAsync([id], cancellationToken);
        if (url == null)
            return false;

        var user = await _userManager.GetUserAsync(userPrincipal);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        // Logic: Admin can delete all. Users can only delete their own.
        if (!isAdmin && url.CreatedBy != user.UserName)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this URL.");
        }

        _context.ShortUrls.Remove(url);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private string GenerateShortCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[6];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }
}