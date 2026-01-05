using CarListingApp.DAL.DBContext;
using CarListingApp.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.CarService;

public class CarService : ICarService
{
    private readonly CarListingContext _context;

    public CarService(CarListingContext context)
    {
        _context = context;
    }

    public async Task<List<CarDto>> GetAll(CancellationToken cancellationToken)
    {
        var cars = await _context.Cars
            .Include(c => c.StatusNavigation)
            .ToListAsync(cancellationToken);
        var carDtos = new List<CarDto>();

        foreach (var car in cars)
        {
            carDtos.Add(new CarDto
            {
                Price = car.Price,
                Brand = car.Brand,
                Model = car.Model,
                Color = car.Color,
                Year = car.Year,
                Vin = car.Vin,
                EngineDisplacement = car.EngineDisplacement,
                EnginePower = car.EnginePower,
                Mileage = car.Mileage,
                Status = car.StatusNavigation.StatusName,
                Description = car.Description,
            });
        }

        return carDtos;
    }

    public async Task<CarDto> GetById(int? id, CancellationToken cancellationToken)
    {
        if (id == null || id < 0)
            throw new ArgumentException("ID must be valid.");
        
        var car = await _context.Cars
            .Include(c => c.StatusNavigation)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car == null)
            throw new KeyNotFoundException($"No car with ID {id} found.");

        return new CarDto
        {
            Price = car.Price,
            Brand = car.Brand,
            Model = car.Model,
            Color = car.Color,
            Year = car.Year,
            Vin = car.Vin,
            EngineDisplacement = car.EngineDisplacement,
            EnginePower = car.EnginePower,
            Mileage = car.Mileage,
            Status = car.StatusNavigation.StatusName,
            Description = car.Description
        };
    }
}