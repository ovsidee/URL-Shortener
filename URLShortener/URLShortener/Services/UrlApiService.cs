using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Extensions;
using URLShortener.Models;
using URLShortener.Models.DTO.Response;

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

    public async Task<IEnumerable<UrlResponseDto>> GetAllUrlsAsync(CancellationToken cancellationToken)
    {
        var entities = await _context.ShortUrls.ToListAsync(cancellationToken);
        return entities.Select(u => u.ToDto()); 
    }

    public async Task<UrlResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.ShortUrls.FindAsync([id], cancellationToken);
        return entity?.ToDto();
    }

    public async Task<UrlResponseDto?> GetByPathAsync(string shortCode, CancellationToken cancellationToken)
    {
        var entity = await _context.ShortUrls
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<UrlResponseDto> AddUrlAsync(string originalUrl, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken)
    {
        // check if exists
        var existing = await _context.ShortUrls
            .FirstOrDefaultAsync(u => u.OriginalURL == originalUrl, cancellationToken);

        if (existing != null)
            throw new InvalidOperationException("This URL already exists.");

        var user = await _userManager.GetUserAsync(userPrincipal);
        if (user == null) throw new UnauthorizedAccessException("User is not authenticated.");

        var shortUrl = new ShortUrl
        {
            OriginalURL = originalUrl,
            CreatedBy = user.UserName!,
            CreatedDate = DateTime.UtcNow,
            ShortCode = GenerateShortCode()
        };

        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync(cancellationToken);

        return shortUrl.ToDto(); 
    }

    public async Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken)
    {
        var url = await _context.ShortUrls.FindAsync([id], cancellationToken);
        if (url == null) return false;

        var user = await _userManager.GetUserAsync(userPrincipal);
        if (user == null) throw new UnauthorizedAccessException("User not found");
        
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && url.CreatedBy != user.UserName)
            throw new UnauthorizedAccessException("Not allowed");

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