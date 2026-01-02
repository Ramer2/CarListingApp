using System.Globalization;
using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Services.DTOs;
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

    public async Task<List<UserDto>> GetAll(CancellationToken cancellationToken)
    {
        var userDtos = new List<UserDto>();
        var users = await _context.Users
                    .Include(u => u.RoleNavigation)
                    .Include(u => u.Cars)
                    .ThenInclude(c => c.StatusNavigation)
                    .ToListAsync();
        
        foreach (var user in users)
        {
            var listedCars = new List<CarDto>();

            foreach (var car in user.Cars)
            {
                listedCars.Add(new CarDto
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
            
            userDtos.Add(new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
                IsBlocked = user.IsBlocked,
                Role = user.RoleNavigation.RoleName,
                ListedCars = listedCars,
            });
        }
        
        return userDtos;
    }

    public async Task<UserDto> GetUserById(int? id, CancellationToken cancellationToken)
    {
        if (id == null || id < 0)
            throw new ArgumentException("Invalid ID.");

        var user = await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.RoleNavigation)
            .Include(u => u.Cars)
            .ThenInclude(c => c.StatusNavigation)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");
        
        var listedCars = new List<CarDto>();
        foreach (var car in user.Cars)
        {
            listedCars.Add(new CarDto
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

        return new UserDto
        {
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = user.IsBlocked,
            Role = user.RoleNavigation.RoleName,
            ListedCars = listedCars,
        };
    }

    public async Task<UserDto> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var emailCheck = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(createUserDto.Email));
        if (emailCheck != null)
            throw new ArgumentException("An account with this email already exists.");

        var usernameCheck = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(createUserDto.Username));
        if (usernameCheck != null)
            throw new ArgumentException("This username is already taken.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.Equals(createUserDto.RoleName));
        if (role == null)
            throw new KeyNotFoundException($"No role with name {createUserDto.RoleName} found.");

        var newUser = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            CreatedAt =  DateTime.Now,
            IsBlocked = false,
            RoleNavigation = role
        };
        
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, createUserDto.Password);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new UserDto
        {
            Username = newUser.Username,
            Email = newUser.Email,
            CreatedAt = newUser.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = newUser.IsBlocked,
            Role = role.RoleName,
            ListedCars = new List<CarDto>()
        };
    }
}