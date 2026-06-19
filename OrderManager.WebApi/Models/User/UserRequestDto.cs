using System.ComponentModel.DataAnnotations;

namespace OrderManager.WebApi.Models.User;

public class UserRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
