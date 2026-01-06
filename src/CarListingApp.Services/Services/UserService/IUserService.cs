using CarListingApp.Services.DTOs.User;

namespace CarListingApp.Services.Services.UserService;

public interface IUserService
{
    public Task<List<UserDto>> GetAll(CancellationToken cancellationToken);
    
    // for Admin
    public Task<UserDto> GetUserById(int? id, CancellationToken cancellationToken);
    
    // for User and Dealer (additional checks)
    public Task<UserDto> GetUserByEmail(string email, CancellationToken cancellationToken);
    
    public Task<UserDto> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken);
    
    public Task<UserDto> UpdateUser(CreateUserDto updateUserDto, int id, CancellationToken cancellationToken);
    
    public Task DeleteUser(int? id, CancellationToken cancellationToken);
}