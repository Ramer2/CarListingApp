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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user == null)
            throw new AccessViolationException("Invalid credentials.");
        return user;
    }

    public async Task<List<UserDto>> GetAll(CancellationToken cancellationToken)
    {
        var userDtos = new List<UserDto>();
        var users = await _context.Users
                    .Include(u => u.Cars)
                    .ToListAsync(cancellationToken);
        
        foreach (var user in users)
        {
            var listedCars = new List<CarDto>();

            foreach (var car in user.Cars)
            {
                listedCars.Add(new CarDto
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
                    Status = (StatusEnum) car.Status,
                    Description = car.Description,
                });
            }
            
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
                IsBlocked = user.IsBlocked,
                Role = (RolesEnum) user.Role,
                ListedCars = listedCars,
            });
        }
        
        return userDtos;
    }

    public async Task<UserDto> GetUserById(int id, string requesterEmail, CancellationToken cancellationToken)
    {
        if (id < 0)
            throw new ArgumentException("Invalid ID.");

        var requester = await GetRequester(requesterEmail, cancellationToken);

        if ((RolesEnum)requester.Role != RolesEnum.Admin)
        {
            if (requester.Id != id)
                throw new AccessViolationException();
        }

        var user = await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");
        
        var listedCars = new List<CarDto>();
        foreach (var car in user.Cars)
        {
            listedCars.Add(new CarDto
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
                Status = (StatusEnum) car.Status,
                Description = car.Description,
            });
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = user.IsBlocked,
            Role = (RolesEnum) user.Role,
            ListedCars = listedCars,
        };
    }

    public async Task<UserDto> GetUserByEmail(string email, string requesterEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email must be valid.");

        var requester = await GetRequester(requesterEmail, cancellationToken);

        if ((RolesEnum)requester.Role != RolesEnum.Admin)
        {
            if (!requester.Email.Equals(email))
                throw new AccessViolationException();
        }
        
        var user = await _context.Users
            .Where(u => u.Email.Equals(email))
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with email {email} not found.");
        
        var listedCars = new List<CarDto>();
        foreach (var car in user.Cars)
        {
            listedCars.Add(new CarDto
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
                Status = (StatusEnum) car.Status,
                Description = car.Description,
            });
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = user.IsBlocked,
            Role = (RolesEnum) user.Role,
            ListedCars = listedCars,
        };
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
        
        var emailCheck = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(createUserDto.Email), cancellationToken);
        if (emailCheck != null)
            throw new ArgumentException("An account with this email already exists.");
        
        var usernameCheck = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(createUserDto.Username), cancellationToken);
        if (usernameCheck != null)
            throw new ArgumentException("This username is already taken.");

        var newUser = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            CreatedAt =  DateTime.Now,
            IsBlocked = false,
            Role = (int) createUserDto.Role
        };
        
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, createUserDto.Password);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Equals(createUserDto.Email), cancellationToken);
        
        if (user == null)
            throw new KeyNotFoundException($"User with email {createUserDto.Email} not found.");
        
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = user.IsBlocked,
            Role = (RolesEnum) user.Role,
            ListedCars = new List<CarDto>()
        };
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

        if (!user.Email.Equals(updateUserDto.Email))
        {
            var emailCheck = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(updateUserDto.Email), cancellationToken);
            if (emailCheck != null)
                throw new ArgumentException("An account with this email already exists.");
        }
        
        if (!user.Username.Equals(updateUserDto.Username))
        {
            var emailCheck = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(updateUserDto.Username), cancellationToken);
            if (emailCheck != null)
                throw new ArgumentException("An account with this username already exists.");
        }
        
        user.Email = updateUserDto.Email;
        user.Username = updateUserDto.Username;
        if (updateUserDto.Password != null)
            user.PasswordHash = _passwordHasher.HashPassword(user, updateUserDto.Password);
        
        user.Role = (int) updateUserDto.Role;
        if (user.IsBlocked != updateUserDto.IsBlocked) user.IsBlocked = updateUserDto.IsBlocked;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        var listedCarsDto = new List<CarDto>();
        foreach (var car in user.Cars)
        {
            listedCarsDto.Add(new CarDto
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
                Status = (StatusEnum) car.Status,
                Description = car.Description,
            });
        }
        
        return new UserDto
        {
            Id = user.Id,
            Username = updateUserDto.Username,
            Email = updateUserDto.Email,
            CreatedAt = user.CreatedAt.ToString(CultureInfo.InvariantCulture),
            IsBlocked = updateUserDto.IsBlocked,
            Role = updateUserDto.Role,
            ListedCars = listedCarsDto
        };
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
        {
            _context.UserFavorites.Remove(carFav);
        }

        foreach (var car in user.Cars)
        {
            foreach (var serviceRecord in car.ServiceRecords)
            {
                _context.ServiceRecords.Remove(serviceRecord);
            }
            _context.Cars.Remove(car);
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}