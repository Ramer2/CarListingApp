namespace CarListingApp.Services.Exceptions.Car;

public class CarNotFoundException : Exception
{
    public CarNotFoundException()
    {
    }

    public CarNotFoundException(string? message) : base(message)
    {
    }
}