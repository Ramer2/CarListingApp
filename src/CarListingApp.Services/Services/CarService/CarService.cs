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
    
    // helper methods
    private async Task<User> GetRequester(string email, CancellationToken ct)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user == null)
            throw new UnauthorizedAccessException();
        return user;
    }
    
    private static CarDto ToDto(Car car) => new()
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
        Status = (StatusEnum)car.Status,
        Description = car.Description
    };

    public async Task<List<CarDto>> GetAll(CancellationToken cancellationToken)
    {
        var cars = await _context.Cars
            .AsNoTracking()
            .Include(c => c.SellerNavigation)
            .ToListAsync(cancellationToken);
        var carDtos = new List<CarDto>();

        foreach (var car in cars)
        {
            carDtos.Add(ToDto(car));
        }

        return carDtos;
    }

    public async Task<CarDto> GetById(int? id, CancellationToken cancellationToken)
    {
        if (id == null || id < 0)
            throw new ArgumentException("ID must be valid.");
        
        var car = await _context.Cars
            .AsNoTracking()
            .Include(c => c.SellerNavigation)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car == null)
            throw new KeyNotFoundException($"No car with ID {id} found.");

        return ToDto(car);
    }

    public async Task<CarDto> CreateCar(CreateCarDto createCarDto, string sellerEmail, CancellationToken cancellationToken)
    {
        var seller = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(sellerEmail), cancellationToken);
        
        if (seller == null)
            throw new ArgumentException("No such seller was found.");

        var activeCars = await _context.Cars.AsNoTracking().CountAsync(
            c => c.Seller == seller.Id && c.Status == (int)StatusEnum.Active,
            cancellationToken);
        if (seller.Role == (int) RolesEnum.User && activeCars > 0)
            throw new ArgumentException("User cannot sell more than one car at a time.");

        if (createCarDto.Vin != null)
        {
            var vinExists = await _context.Cars
                .AsNoTracking()
                .AnyAsync(c => c.Vin == createCarDto.Vin, cancellationToken);
            
            if (vinExists)
                throw new ArgumentException("Another car has this Vin."); 
        }
        
        var status = createCarDto.Status ?? StatusEnum.Active;
        
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
            Status = (int) status,
            Description = createCarDto.Description,
        };
        
        _context.Cars.Add(newCar);
        await _context.SaveChangesAsync(cancellationToken);
        
        return ToDto(newCar);
    }

    public async Task<CarDto> UpdateCar(CreateCarDto createCarDto, int id, string requesterEmail, CancellationToken cancellationToken)
    {
        var requester = await GetRequester(requesterEmail, cancellationToken);

        if (requester == null)
            throw new ArgumentException("No such requester was found.");

        var car = await _context.Cars
            .Include(c => c.SellerNavigation)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car == null)
            throw new ArgumentException("Car not found.");

        if (car.SellerNavigation.Id != requester.Id && !(requester.Role == (int)RolesEnum.Admin))
            throw new ArgumentException("Seller is not allowed to update this car.");
        
        var status = createCarDto.Status ?? StatusEnum.Active;

        if (requester.Role == (int) RolesEnum.User)
        {
            var activeCars = await _context.Cars.AsNoTracking().CountAsync(
                c => c.Seller == requester.Id
                     && c.Status == (int)StatusEnum.Active
                     && c.Id != car.Id,
                cancellationToken);

            if (activeCars > 0 && createCarDto.Status == StatusEnum.Active)
                throw new ArgumentException("User cannot have more than one active car.");
        }

        if (!string.IsNullOrWhiteSpace(createCarDto.Vin))
        {
            var vinExists = await _context.Cars.AsNoTracking().AnyAsync(
                c => c.Vin == createCarDto.Vin && c.Id != car.Id,
                cancellationToken);

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
        car.Status = (int) status;

        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(car);
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

        var requester = await GetRequester(requesterEmail, cancellationToken);

        if (requester.Role != (int)RolesEnum.Admin &&
            car.SellerNavigation.Id != requester.Id)
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