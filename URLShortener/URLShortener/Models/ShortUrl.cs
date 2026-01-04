using System.ComponentModel.DataAnnotations;

namespace URLShortener.Models;

public class ShortUrl
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string OriginalURL { get; set; }
    
    public string? ShortCode { get; set; } //because it's generated later

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public required string CreatedBy { get; set; }
}