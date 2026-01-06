using CarListingApp.Services.DTOs.Car;

namespace CarListingApp.Services.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string CreatedAt { get; set; }

    public bool IsBlocked { get; set; }

    public string Role { get; set; }

    public List<CarDto> ListedCars { get; set; } = new();
}