using System.ComponentModel.DataAnnotations;
using CarListingApp.Models.Models.Enums;

namespace CarListingApp.Services.DTOs.Car;

public class CreateCarDto
{
    [Required]
    [Range(1, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Brand { get; set; } = null!;

    [Required]
    public string Model { get; set; } = null!;

    public string? Color { get; set; }

    [Required]
    [Range(1900, 2029)]
    public int Year { get; set; }
    
    public string? Vin { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? EngineDisplacement { get; set; }
    
    [Range(0, double.MaxValue)]
    public int? EnginePower { get; set; }

    [Range(0, double.MaxValue)]
    public int? Mileage { get; set; }

    public string? Description { get; set; }
    
    public StatusEnum? Status { get; set; }
}