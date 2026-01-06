using CarListingApp.Services.DTOs.Car;

namespace CarListingApp.Services.Services.Favorite;

public interface IFavoritesService
{
    Task AddToFavorites(int carId, string userEmail, CancellationToken cancellationToken);
    Task RemoveFromFavorites(int carId, string userEmail, CancellationToken cancellationToken);
    Task<List<CarDto>> GetFavorites(string userEmail, CancellationToken cancellationToken);
}