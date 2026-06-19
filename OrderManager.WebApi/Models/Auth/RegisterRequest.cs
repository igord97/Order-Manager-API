using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderManager.WebApi.Models.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DefaultValue("")]
    [MinLength(8)]
    [MaxLength(100)]
    [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
    public string Password { get; set; } = string.Empty;
}