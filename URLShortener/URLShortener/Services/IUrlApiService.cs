using System.Security.Claims;
using URLShortener.Models.DTO.Response; 

namespace URLShortener.Services;

public interface IUrlApiService
{
    Task<IEnumerable<UrlResponseDto>> GetAllUrlsAsync(CancellationToken token);
    Task<UrlResponseDto?> GetByIdAsync(int id, CancellationToken token);
    Task<UrlResponseDto> AddUrlAsync(string originalUrl, ClaimsPrincipal user, CancellationToken token);
    Task<bool> DeleteUrlAsync(int id, ClaimsPrincipal user, CancellationToken token);
    Task<UrlResponseDto?> GetByPathAsync(string shortCode, CancellationToken token);
}