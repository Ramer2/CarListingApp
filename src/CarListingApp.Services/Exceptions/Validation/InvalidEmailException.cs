namespace CarListingApp.Services.Exceptions.Validation;

public class InvalidEmailException : Exception
{
    public InvalidEmailException()
    {
    }

    public InvalidEmailException(string? message) : base(message)
    {
    }
}