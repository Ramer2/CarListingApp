using CarListingApp.Models.Models;

namespace CarListingApp.Services.DTOs;

public class CarDto
{
    public int Id { get; set; }
    
    public double Price { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? Color { get; set; }

    public int Year { get; set; }

    public string? Vin { get; set; }

    public double? EngineDisplacement { get; set; }

    public double? EnginePower { get; set; }

    public int? Mileage { get; set; }

    public string Status { get; set; }

    public string? Description { get; set; }
}