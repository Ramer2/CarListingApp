using CarListingApp.Services.DTOs.ServiceRecord;

namespace CarListingApp.Services.Services.ServiceRecord;

public interface IServiceRecordsService
{
    Task<List<ServiceRecordDto>> GetAll(int carId, CancellationToken cancellationToken);

    Task<ServiceRecordDto> GetById(int carId, int recordId, CancellationToken cancellationToken);

    Task<ServiceRecordDto> CreateServiceRecord(int carId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken);

    Task<ServiceRecordDto> UpdateServiceRecord(int carId, int recordId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken);

    Task DeleteServiceRecord(int carId, int recordId, string requesterEmail, CancellationToken cancellationToken);
}