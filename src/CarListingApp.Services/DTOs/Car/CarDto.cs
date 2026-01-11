using CarListingApp.Models.Models.Enums;

namespace CarListingApp.Services.DTOs.Car;

public class CarDto
{
    public int Id { get; set; }
    
    public decimal Price { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? Color { get; set; }

    public int Year { get; set; }

    public string? Vin { get; set; }

    public decimal? EngineDisplacement { get; set; }

    public int? EnginePower { get; set; }

    public int? Mileage { get; set; }
    
    public int SellerId { get; set; }

    public StatusEnum Status { get; set; }

    public string? Description { get; set; }
}