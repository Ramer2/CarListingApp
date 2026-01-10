using CarListingApp.Services.DTOs.User;

namespace CarListingApp.Services.Services.UserService;

public interface IUserService
{
    public Task<List<UserDto>> GetAll(CancellationToken cancellationToken);
    public Task<UserDto> GetUserById(int id, string requesterEmail, CancellationToken cancellationToken);
    public Task<UserDto> GetUserByEmail(string email, string requesterEmail, CancellationToken cancellationToken);
    public Task<UserDto> CreateUser(CreateUserDto createUserDto, string? requesterEmail, CancellationToken cancellationToken);
    public Task<UserDto> UpdateUser(CreateUserDto updateUserDto, int id, string requesterEmail, CancellationToken cancellationToken);
    public Task DeleteUser(int id, string requesterEmail, CancellationToken cancellationToken);
}