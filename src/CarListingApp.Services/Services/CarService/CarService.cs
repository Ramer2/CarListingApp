using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Models.Models.Enums;
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
            .Include(c => c.SellerNavigation)
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
                SellerId = car.SellerNavigation.Id,
                Status = (StatusEnum) car.Status,
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
            .Include(c => c.SellerNavigation)
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
            SellerId = car.SellerNavigation.Id,
            Status = (StatusEnum) car.Status,
            Description = car.Description
        };
    }

    public async Task<CarDto> CreateCar(CreateCarDto createCarDto, string sellerEmail, CancellationToken cancellationToken)
    {
        var seller = await _context.Users
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(u => u.Email.Equals(sellerEmail), cancellationToken);
        
        if (seller == null)
            throw new ArgumentException("No such seller was found.");

        var activeCars = await _context.Cars.CountAsync(
            c => c.Seller == seller.Id && c.Status == (int)StatusEnum.Active,
            cancellationToken);
        if (seller.Role == (int) RolesEnum.User && activeCars > 0)
            throw new ArgumentException("User cannot sell more than one car at a time.");

        if (createCarDto.Vin != null)
        {
            var vinExists = await _context.Cars
                .AnyAsync(c => c.Vin == createCarDto.Vin, cancellationToken);
            
            if (vinExists)
                throw new ArgumentException("Another car has this Vin."); 
        }
        
        if (createCarDto.Status == null)
            createCarDto.Status = StatusEnum.Active;
        
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
            SellerNavigation = seller,
            Status = (int) createCarDto.Status,
            Description = createCarDto.Description,
        };
        
        _context.Cars.Add(newCar);
        await _context.SaveChangesAsync(cancellationToken);
        
        var car = await _context.Cars
            .Include(c => c.SellerNavigation)
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
            SellerId = car.SellerNavigation.Id,
            Status = (StatusEnum) car.Status,
            Description = car.Description,
        };
    }

    public async Task<CarDto> UpdateCar(CreateCarDto createCarDto, int id, string sellerEmail, CancellationToken cancellationToken)
    {
        var seller = await _context.Users
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(u => u.Email == sellerEmail, cancellationToken);

        if (seller == null)
            throw new ArgumentException("No such seller was found.");

        var car = await _context.Cars
            .Include(c => c.SellerNavigation)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car == null)
            throw new ArgumentException("Car not found.");

        if (car.SellerNavigation.Id != seller.Id && !(seller.Role == (int)RolesEnum.Admin))
            throw new ArgumentException("Seller is not allowed to update this car.");
        
        if (createCarDto.Status == null)
            createCarDto.Status = StatusEnum.Active;

        if (seller.Role == (int) RolesEnum.User)
        {
            var activeCars = await _context.Cars.CountAsync(
                c => c.Seller == seller.Id && c.Status == (int)StatusEnum.Active,
                cancellationToken);

            if (activeCars > 0 && createCarDto.Status == StatusEnum.Active)
                throw new ArgumentException("User cannot have more than one active car.");
        }

        if (!string.IsNullOrWhiteSpace(createCarDto.Vin))
        {
            var vinExists = await _context.Cars
                .AnyAsync(c => c.Vin == createCarDto.Vin, cancellationToken);

            if (vinExists)
                throw new ArgumentException("Another car has this VIN.");
        }
        
        car.Price = createCarDto.Price;
        car.Brand = createCarDto.Brand;
        car.Model = createCarDto.Model;
        car.Color = createCarDto.Color;
        car.Year = createCarDto.Year;
        car.Vin = createCarDto.Vin;
        car.EngineDisplacement = createCarDto.EngineDisplacement;
        car.EnginePower = createCarDto.EnginePower;
        car.Mileage = createCarDto.Mileage;
        car.Description = createCarDto.Description;
        car.Status = (int) createCarDto.Status;

        await _context.SaveChangesAsync(cancellationToken);

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
            SellerId = car.SellerNavigation.Id,
            Status = (StatusEnum) car.Status,
            Description = car.Description
        };
    }

    public async Task DeleteCar(int id, string requesterEmail, CancellationToken cancellationToken)
    {
        var car = await _context.Cars
            .Include(c => c.SellerNavigation)
            .Include(c => c.UserFavorites)
            .Include(c => c.ServiceRecords)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car == null)
            throw new KeyNotFoundException("Car not found.");

        var seller = car.SellerNavigation;
        if (seller.Role != (int)RolesEnum.Admin && !seller.Email.Equals(requesterEmail)) 
            throw new UnauthorizedAccessException();

        foreach (var uf in car.UserFavorites)
        {
            _context.UserFavorites.Remove(uf);
        }

        foreach (var sr in car.ServiceRecords)
        {
            _context.ServiceRecords.Remove(sr);
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync(cancellationToken);
    }
}