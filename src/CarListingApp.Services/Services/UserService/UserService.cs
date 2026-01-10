using System.Globalization;
using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Models.Models.Enums;
using CarListingApp.Services.DTOs.Car;
using CarListingApp.Services.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.UserService;

public class UserService : IUserService
{
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly CarListingContext _context;

    public UserService(CarListingContext context)
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
            throw new AccessViolationException("Invalid credentials.");
        return user;
    }
    
    private static UserDto ToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = user.IsBlocked,
            Role = (RolesEnum) user.Role,
            ListedCars = user.Cars?.Select(ToCarDto).ToList() ?? new List<CarDto>()
        };
    }
    
    private static CarDto ToCarDto(Car car) => new()
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

    public async Task<List<UserDto>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt.ToString(CultureInfo.InvariantCulture),
                IsBlocked = u.IsBlocked,
                Role = (RolesEnum)u.Role,
                ListedCars = u.Cars.Select(c => new CarDto
                {
                    Id = c.Id,
                    Price = c.Price,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    Year = c.Year,
                    Vin = c.Vin,
                    EngineDisplacement = c.EngineDisplacement,
                    EnginePower = c.EnginePower,
                    Mileage = c.Mileage,
                    Status = (StatusEnum)c.Status,
                    Description = c.Description,
                    SellerId = c.SellerNavigation.Id
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<UserDto> GetUserById(int id, string requesterEmail, CancellationToken cancellationToken)
    {
        var requester = await GetRequester(requesterEmail, cancellationToken);
        if ((RolesEnum)requester.Role != RolesEnum.Admin && requester.Id != id)
            throw new AccessViolationException();

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt.ToString(CultureInfo.InvariantCulture),
                IsBlocked = u.IsBlocked,
                Role = (RolesEnum)u.Role,
                ListedCars = u.Cars.Select(c => new CarDto
                {
                    Id = c.Id,
                    Price = c.Price,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    Year = c.Year,
                    Vin = c.Vin,
                    EngineDisplacement = c.EngineDisplacement,
                    EnginePower = c.EnginePower,
                    Mileage = c.Mileage,
                    Status = (StatusEnum)c.Status,
                    Description = c.Description,
                    SellerId = c.SellerNavigation.Id
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        return user;
    }

    public async Task<UserDto> GetUserByEmail(string email, string requesterEmail, CancellationToken cancellationToken)
    {
        var requester = await GetRequester(requesterEmail, cancellationToken);
        if ((RolesEnum)requester.Role != RolesEnum.Admin && !requester.Email.Equals(email))
            throw new AccessViolationException();

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt.ToString(CultureInfo.InvariantCulture),
                IsBlocked = u.IsBlocked,
                Role = (RolesEnum)u.Role,
                ListedCars = u.Cars.Select(c => new CarDto
                {
                    Id = c.Id,
                    Price = c.Price,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    Year = c.Year,
                    Vin = c.Vin,
                    EngineDisplacement = c.EngineDisplacement,
                    EnginePower = c.EnginePower,
                    Mileage = c.Mileage,
                    Status = (StatusEnum)c.Status,
                    Description = c.Description,
                    SellerId = c.SellerNavigation.Id
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with email {email} not found.");

        return user;
    }

    public async Task<UserDto> CreateUser(CreateUserDto createUserDto, string? requesterEmail, CancellationToken cancellationToken)
    {
        if (createUserDto.Role == RolesEnum.Admin && requesterEmail != null)
        {
            var requester = await GetRequester(requesterEmail, cancellationToken);
            if ((RolesEnum)requester.Role != RolesEnum.Admin)
                throw new AccessViolationException();
        }
        
        if (string.IsNullOrEmpty(createUserDto.Password))
            throw new ArgumentException("Password must be valid.");
        
        if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email, cancellationToken))
            throw new ArgumentException("An account with this email already exists.");
        if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username, cancellationToken))
            throw new ArgumentException("This username is already taken.");

        var newUser = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            CreatedAt = DateTime.Now,
            IsBlocked = false,
            Role = (int) createUserDto.Role,
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, createUserDto.Password);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        return ToUserDto(newUser);
    }

    public async Task<UserDto> UpdateUser(CreateUserDto updateUserDto, int id, string requesterEmail, CancellationToken cancellationToken)
    {
        var requester = await GetRequester(requesterEmail, cancellationToken);
        if ((RolesEnum)requester.Role != RolesEnum.Admin)
        {
            if (requester.Id != id
                || requester.Role != (int) updateUserDto.Role
                || requester.IsBlocked != updateUserDto.IsBlocked)
                throw new AccessViolationException();
        }

        var user = await _context.Users
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"No user found with ID {id}.");

        if (!user.Email.Equals(updateUserDto.Email) &&
            await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email, cancellationToken))
            throw new ArgumentException("An account with this email already exists.");

        if (!user.Username.Equals(updateUserDto.Username) &&
            await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username, cancellationToken))
            throw new ArgumentException("An account with this username already exists.");

        user.Email = updateUserDto.Email;
        user.Username = updateUserDto.Username;
        if (!string.IsNullOrEmpty(updateUserDto.Password))
            user.PasswordHash = _passwordHasher.HashPassword(user, updateUserDto.Password);
        user.Role = (int) updateUserDto.Role;
        user.IsBlocked = updateUserDto.IsBlocked;

        await _context.SaveChangesAsync(cancellationToken);

        return ToUserDto(user);
    }

    public async Task DeleteUser(int id, string requesterEmail, CancellationToken cancellationToken)
    {
        var requester = await GetRequester(requesterEmail, cancellationToken);
        if ((RolesEnum)requester.Role != RolesEnum.Admin && requester.Id != id)
            throw new AccessViolationException();
        
        var user = await _context.Users
            .Include(u => u.UserFavorites)
            .Include(u => u.Cars)
            .ThenInclude(c => c.ServiceRecords)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"No user found with ID {id}.");

        foreach (var carFav in user.UserFavorites)
            _context.UserFavorites.Remove(carFav);

        foreach (var car in user.Cars)
        {
            foreach (var serviceRecord in car.ServiceRecords)
                _context.ServiceRecords.Remove(serviceRecord);
            
            _context.Cars.Remove(car);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}