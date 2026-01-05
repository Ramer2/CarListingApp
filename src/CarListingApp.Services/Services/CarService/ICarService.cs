using CarListingApp.Services.DTOs.Car;

namespace CarListingApp.Services.Services.CarService;

public interface ICarService
{
    public Task<List<CarDto>> GetAll(CancellationToken cancellationToken);
    
    public Task<CarDto> GetById(int? id, CancellationToken cancellationToken);
    
    public Task<CarDto> CreateCar(CreateCarDto createCarDto, CancellationToken cancellationToken);
}