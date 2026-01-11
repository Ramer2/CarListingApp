namespace CarListingApp.Services.Exceptions.User;

// user for the given data was not found
public class UserNotFoundException : Exception
{
    public UserNotFoundException()
    {
    }

    public UserNotFoundException(string? message) : base(message)
    {
    }
}