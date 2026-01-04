namespace URLShortener.Models.DTO.Response;

public class UrlResponseDto
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty; 
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}