namespace CarListingApp.Services.Exceptions.User;

public class UserSellLimitException : Exception
{
    public UserSellLimitException()
    {
    }

    public UserSellLimitException(string? message) : base(message)
    {
    }
}