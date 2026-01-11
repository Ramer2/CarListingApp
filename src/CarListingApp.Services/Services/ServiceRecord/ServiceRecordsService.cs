using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models.Enums;
using CarListingApp.Services.DTOs.ServiceRecord;
using CarListingApp.Services.Exceptions.Auth;
using CarListingApp.Services.Exceptions.Car;
using CarListingApp.Services.Exceptions.Favorite;
using CarListingApp.Services.Exceptions.Record;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.ServiceRecord;

public class ServiceRecordsService : IServiceRecordsService
{
    private readonly CarListingContext _context;

    public ServiceRecordsService(CarListingContext context)
    {
        _context = context;
    }
    
    // helper methods
    private static ServiceRecordDto ToDto(Models.Models.ServiceRecord sr)
    {
        return new ServiceRecordDto
        {
            Id = sr.Id,
            MileageAtService = sr.MileageAtService,
            ServiceDate = sr.ServiceDate.ToShortDateString(),
            Grade = sr.Grade,
            CarId = sr.Car
        };
    }

    public async Task<List<ServiceRecordDto>> GetAll(int carId, CancellationToken cancellationToken)
    {
        return await _context.ServiceRecords
            .AsNoTracking()
            .Where(sr => sr.Car == carId)
            .Select(sr => ToDto(sr))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceRecordDto> GetById(int carId, int recordId, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(sr => sr.Id == recordId && sr.Car == carId, cancellationToken);

        if (record == null)
            throw new RecordNotFoundException($"No service record with ID {recordId} found for this car.");

        return ToDto(record);
    }

    public async Task<ServiceRecordDto> CreateServiceRecord(int carId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken)
    {
        var car = await _context.Cars
                    .AsNoTracking()
                    .Where(c => c.Id == carId)
                    .Select(c => new { c.Id, SellerId = c.SellerNavigation.Id })
                    .FirstOrDefaultAsync(cancellationToken)
                        ?? throw new CarNotFoundException("Car not found.");

        var requester = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Email == requesterEmail)
                    .Select(u => new { u.Id, u.Role })
                    .FirstOrDefaultAsync(cancellationToken)
                        ?? throw new AuthorizationFailedException();

        if (car.SellerId != requester.Id && requester.Role != (int) RolesEnum.Admin)
            throw new AuthorizationFailedException();

        var record = new Models.Models.ServiceRecord
        {
            Car = carId,
            MileageAtService = dto.MileageAtService,
            ServiceDate = dto.ServiceDate,
            Grade = dto.Grade
        };

        _context.ServiceRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(record);
    }

    public async Task<ServiceRecordDto> UpdateServiceRecord(int carId, int recordId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
                      .Where(sr => sr.Id == recordId && sr.Car == carId)
                      .FirstOrDefaultAsync(cancellationToken)
                  ?? throw new RecordNotFoundException("Service record not found.");

        var requester = await _context.Users
                        .AsNoTracking()
                        .Where(u => u.Email == requesterEmail)
                        .Select(u => new { u.Id, u.Role })
                        .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new AuthorizationFailedException();

        var car = await _context.Cars
                      .AsNoTracking()
                      .Where(c => c.Id == carId)
                      .Select(c => new { c.Id, SellerId = c.SellerNavigation.Id })
                      .FirstOrDefaultAsync(cancellationToken)
                  ?? throw new CarNotFoundException("Car not found.");
        
        if (car.SellerId != requester.Id && requester.Role != (int) RolesEnum.Admin)
            throw new AuthorizationFailedException();

        record.MileageAtService = dto.MileageAtService;
        record.ServiceDate = dto.ServiceDate;
        record.Grade = dto.Grade;

        await _context.SaveChangesAsync(cancellationToken);

        return ToDto(record);
    }

    public async Task DeleteServiceRecord(int carId, int recordId, string requesterEmail, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
                     .Where(sr => sr.Id == recordId && sr.Car == carId)
                     .FirstOrDefaultAsync(cancellationToken)
                 ?? throw new RecordNotFoundException("Service record not found.");

        var requester = await _context.Users
                        .AsNoTracking()
                        .Where(u => u.Email == requesterEmail)
                        .Select(u => new { u.Id, u.Role })
                        .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new AuthorizationFailedException();

        var car = await _context.Cars
                      .AsNoTracking()
                      .Where(c => c.Id == carId)
                      .Select(c => new { c.Id, SellerId = c.SellerNavigation.Id })
                      .FirstOrDefaultAsync(cancellationToken)
                  ?? throw new CarNotFoundException("Car not found.");
        
        if (!(requester.Role == (int) RolesEnum.Admin) && car.SellerId != requester.Id)
            throw new AuthorizationFailedException();

        _context.ServiceRecords.Remove(record);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
