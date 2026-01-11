namespace CarListingApp.Services.Exceptions.User;

// for when the user with this data is already in the system
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException()
    {
    }

    public UserAlreadyExistsException(string? message) : base(message)
    {
    }
}