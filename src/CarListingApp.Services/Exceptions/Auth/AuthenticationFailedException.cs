namespace CarListingApp.Services.Exceptions.Auth;

// wrong password, invalid credentials
public class AuthenticationFailedException : Exception
{
    public  AuthenticationFailedException() {}
    
    public AuthenticationFailedException(string message) : base(message) {}
}