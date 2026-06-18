using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningProject1.Core.DTOs.User;

public class UpdateMyProfileRequestDto
{
    [Required]
    [DefaultValue("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DefaultValue("")]
    public string? OldPassword { get; set; }
    
    [DefaultValue("")]
    public string? NewPassword { get; set; }
}