using System.ComponentModel.DataAnnotations;

namespace CarListingApp.Services.DTOs.User;

public class CreateUserDto
{
    [Required]
    [RegularExpression(@"^[a-z0-9_-]{3,15}$")]
    public string Username { get; set; } = null!;

    [Required]
    [RegularExpression(@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$")]
    public string Password { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;
    
    public bool IsBlocked { get; set; }

}