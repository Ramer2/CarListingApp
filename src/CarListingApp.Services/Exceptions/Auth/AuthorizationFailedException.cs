namespace CarListingApp.Services.Exceptions.Auth;

// user is authenticated but doesn't have permissions to perform the action
public class AuthorizationFailedException : Exception
{
    public  AuthorizationFailedException() {}
    
    public AuthorizationFailedException(string message) : base(message) {}
}