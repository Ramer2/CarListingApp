namespace CarListingApp.Services.Exceptions.Favorite;

public class CarAlreadyFavoritedException : Exception
{
    public CarAlreadyFavoritedException()
    {
    }

    public CarAlreadyFavoritedException(string? message) : base(message)
    {
    }
}