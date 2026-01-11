using CarListingApp.Services.DTOs.Auth;
using CarListingApp.Services.DTOs.User;

namespace CarListingApp.Services.Services.Auth;

public interface IAuthService
{
    public Task<TokenDto> Login(LoginUserDto loginUserDto, CancellationToken cancellationToken);
    
    public Task Register(CreateUserDto createUserDto, CancellationToken cancellationToken);
}