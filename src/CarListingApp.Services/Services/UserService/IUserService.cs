using CarListingApp.Services.DTOs;

namespace CarListingApp.Services.Services.UserService;

public interface IUserService
{
    public Task<List<UserDto>> GetAll(CancellationToken cancellationToken);
    
    public Task<UserDto> GetUserById(int? id, CancellationToken cancellationToken);
}