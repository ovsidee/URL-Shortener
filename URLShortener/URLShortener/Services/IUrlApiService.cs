using System.Security.Claims;
using URLShortener.Models;

namespace URLShortener.Services;

public interface IUrlApiService
{
    Task<IEnumerable<ShortUrl>> GetAllUrlsAsync(CancellationToken cancellationToken);
    Task<ShortUrl?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ShortUrl> AddUrlAsync(string originalUrl, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken);
    Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken);
    Task<ShortUrl?> GetByPathAsync(string shortCode, CancellationToken cancellationToken);
}