namespace CarListingApp.Services.Exceptions.Car;

public class VinDuplicateException : Exception
{
    public VinDuplicateException()
    {
    }

    public VinDuplicateException(string? message) : base(message)
    {
    }
}