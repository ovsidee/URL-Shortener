using System.ComponentModel.DataAnnotations;

namespace URLShortener.Models;

public class AboutContent
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}