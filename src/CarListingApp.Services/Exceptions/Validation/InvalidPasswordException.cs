namespace CarListingApp.Services.Exceptions.Validation;

// for invalid passwords
public class InvalidPasswordException : Exception
{
    public InvalidPasswordException() {}
    
    public InvalidPasswordException(string message) : base(message) {}
}