using CarListingApp.Models.Models;

namespace CarListingApp.Services.DTOs;

public class UserDto
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string CreatedAt { get; set; }

    public bool IsBlocked { get; set; }

    public string Role { get; set; }

    public ICollection<CarDto> ListedCars { get; set; } = new List<CarDto>();
}