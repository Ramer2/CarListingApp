using System.ComponentModel.DataAnnotations;

namespace CarListingApp.Services.DTOs.Car;

public class CreateCarDto
{
    [Required]
    [Range(1, double.MaxValue)]
    public double Price { get; set; }
    
    [Required]
    public string Brand { get; set; } = null!;

    [Required]
    public string Model { get; set; } = null!;

    public string? Color { get; set; }

    [Required]
    [Range(1, 3000)]
    public int Year { get; set; }
    
    public string? Vin { get; set; }
    
    [Range(0, double.MaxValue)]
    public double? EngineDisplacement { get; set; }
    
    [Range(0, double.MaxValue)]
    public double? EnginePower { get; set; }

    [Range(0, double.MaxValue)]
    public int? Mileage { get; set; }

    public int? SellerId { get; set; }

    public string? Description { get; set; }
}