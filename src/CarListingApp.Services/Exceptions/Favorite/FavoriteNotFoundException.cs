namespace CarListingApp.Services.Exceptions.Favorite;

public class FavoriteNotFoundException : Exception
{
    public FavoriteNotFoundException()
    {
    }

    public FavoriteNotFoundException(string? message) : base(message)
    {
    }
}