namespace CarListingApp.Services.Services.TokenService;

public interface ITokenService
{
    public string GenerateToken(string username, string role, string email);
}