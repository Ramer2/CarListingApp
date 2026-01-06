using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Services.DTOs.Car;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.Favorite;

public class FavoritesService : IFavoritesService
{
    private readonly CarListingContext _context;

    public FavoritesService(CarListingContext context)
    {
        _context = context; 
    }
    
    public async Task AddToFavorites(int carId, string userEmail, CancellationToken cancellationToken)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Email == userEmail, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        var carExists = await _context.Cars
            .AnyAsync(c => c.Id == carId, cancellationToken);

        if (!carExists)
            throw new KeyNotFoundException("Car not found.");

        var alreadyFavorited = await _context.UserFavorites
            .AnyAsync(uf => uf.UserId == user.Id && uf.CarId == carId, cancellationToken);

        if (alreadyFavorited)
            throw new ArgumentException("Car already in favorites.");

        _context.UserFavorites.Add(new UserFavorite
        {
            UserId = user.Id,
            CarId = carId
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveFromFavorites(int carId, string userEmail, CancellationToken cancellationToken)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(u => u.Email == userEmail, cancellationToken)
                   ?? throw new KeyNotFoundException("User not found.");

        var favorite = await _context.UserFavorites
            .FirstOrDefaultAsync(
                uf => uf.UserId == user.Id && uf.CarId == carId,
                cancellationToken);

        if (favorite == null)
            throw new KeyNotFoundException("Favorite not found.");

        _context.UserFavorites.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CarDto>> GetFavorites(string userEmail, CancellationToken cancellationToken)
    {
        return await _context.UserFavorites
            .Where(uf => uf.User.Email == userEmail)
            .Select(uf => new CarDto
            {
                Id = uf.Car.Id,
                Brand = uf.Car.Brand,
                Model = uf.Car.Model,
                Price = uf.Car.Price,
                Mileage = uf.Car.Mileage,
                Year = uf.Car.Year,
                Status = uf.Car.StatusNavigation.StatusName
            })
            .ToListAsync(cancellationToken);
    }
}