using URLShortener.Models;
using URLShortener.Models.DTO.Response;

namespace URLShortener.Extensions;

public static class UrlMappingExtensions
{
    // Entity to DTO
    public static UrlResponseDto ToDto(this ShortUrl entity)
    {
        return new UrlResponseDto
        {
            Id = entity.Id,
            OriginalUrl = entity.OriginalURL,
            ShortCode = entity.ShortCode ?? "",
            ShortUrl = $"http://localhost:5152/{entity.ShortCode}", 
            CreatedBy = entity.CreatedBy,
            CreatedDate = entity.CreatedDate
        };
    }
}