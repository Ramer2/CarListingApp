using System.ComponentModel.DataAnnotations;

namespace CarListingApp.Services.DTOs.Auth;

public class LoginUserDto
{
    public string? Email { get; set; }
    
    public string? Username { get; set; }

    [Required]
    public string Password { get; set; } = null!;
}