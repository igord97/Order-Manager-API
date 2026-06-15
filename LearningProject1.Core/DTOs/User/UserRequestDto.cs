using System.ComponentModel.DataAnnotations;

namespace LearningProject1.Core.DTOs.User;

public class UserRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
