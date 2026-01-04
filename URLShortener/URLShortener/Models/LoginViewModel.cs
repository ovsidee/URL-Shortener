using System.ComponentModel.DataAnnotations;

namespace URLShortener.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}