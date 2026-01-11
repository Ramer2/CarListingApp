namespace CarListingApp.Services.Exceptions.Validation;

public class InvalidIdException : Exception
{
    public InvalidIdException()
    {
    }

    public InvalidIdException(string? message) : base(message)
    {
    }
}