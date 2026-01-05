using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Services.DTOs;
using CarListingApp.Services.DTOs.Car;
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
                Id = car.Id,
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
            Id = car.Id,
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

    public async Task<CarDto> CreateCar(CreateCarDto createCarDto, CancellationToken cancellationToken)
    {
        var seller = await _context.Users
            .Include(u => u.Cars)
            .ThenInclude(c => c.StatusNavigation)
            .Include(u => u.RoleNavigation)
            .FirstOrDefaultAsync(u => u.Id == createCarDto.SellerId, cancellationToken);
        
        if (seller == null)
            throw new ArgumentException("No such seller was found.");

        var activeCars = seller.Cars.Count(c => c.StatusNavigation.StatusName.Equals("Active"));
        if (seller.RoleNavigation.RoleName.Equals("User") && activeCars > 0)
            throw new ArgumentException("User cannot sell more than one car at a time.");

        if (createCarDto.Vin != null)
        {
            var carCheck = await _context.Cars
                .Where(c => c.Vin != null)
                .FirstOrDefaultAsync(c => c.Vin.Equals(createCarDto.Vin), cancellationToken);
            
            if (carCheck != null)
                throw new ArgumentException("Another car has this Vin."); 
        }
        
        var status = await  _context.Statuses
            .FirstOrDefaultAsync(s => s.StatusName.Equals("Active"), cancellationToken);
        
        var newCar = new Car
        {
            Price = createCarDto.Price,
            Brand = createCarDto.Brand,
            Model = createCarDto.Model,
            Color = createCarDto.Color,
            Year = createCarDto.Year,
            Vin = createCarDto.Vin,
            EngineDisplacement = createCarDto.EngineDisplacement,
            EnginePower = createCarDto.EnginePower,
            Mileage = createCarDto.Mileage,
            SellerNavigation =  seller,
            StatusNavigation = status,
            Description = createCarDto.Description,
        };
        
        _context.Cars.Add(newCar);
        await _context.SaveChangesAsync(cancellationToken);
        
        var car = await _context.Cars
            .Include(c => c.StatusNavigation)
            .FirstOrDefaultAsync(c => c.Id == newCar.Id, cancellationToken);

        return new CarDto
        {
            Id = car.Id,
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
        };
    }
}