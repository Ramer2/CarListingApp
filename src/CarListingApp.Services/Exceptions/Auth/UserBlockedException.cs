namespace CarListingApp.Services.Exceptions.Auth;

// user is blocked
public class UserBlockedException : Exception
{
    public  UserBlockedException() {}
    
    public UserBlockedException(string message) : base(message) {}
}