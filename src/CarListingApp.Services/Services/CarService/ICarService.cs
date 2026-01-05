using CarListingApp.Services.DTOs;

namespace CarListingApp.Services.Services.CarService;

public interface ICarService
{
    public Task<List<CarDto>> GetAll(CancellationToken cancellationToken);
    
    public Task<CarDto> GetById(int? id, CancellationToken cancellationToken);
}