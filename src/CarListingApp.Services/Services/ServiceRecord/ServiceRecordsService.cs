using CarListingApp.DAL.DBContext;
using CarListingApp.Services.DTOs.ServiceRecord;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.ServiceRecord;

public class ServiceRecordsService : IServiceRecordsService
{
    private readonly CarListingContext _context;

    public ServiceRecordsService(CarListingContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceRecordDto>> GetAll(int carId, CancellationToken cancellationToken)
    {
        var records = await _context.ServiceRecords
            .Where(sr => sr.Car == carId)
            .ToListAsync(cancellationToken);

        return records.Select(sr => new ServiceRecordDto
        {
            Id = sr.Id,
            MileageAtService = sr.MileageAtService,
            ServiceDate = sr.ServiceDate.ToShortDateString(),
            Grade = sr.Grade,
            CarId = sr.Car
        }).ToList();
    }

    public async Task<ServiceRecordDto> GetById(int carId, int recordId, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
            .FirstOrDefaultAsync(sr => sr.Id == recordId && sr.Car == carId, cancellationToken);

        if (record == null)
            throw new KeyNotFoundException($"No service record with ID {recordId} found for this car.");

        return new ServiceRecordDto
        {
            Id = record.Id,
            MileageAtService = record.MileageAtService,
            ServiceDate = record.ServiceDate.ToShortDateString(),
            Grade = record.Grade,
            CarId = record.Car
        };
    }

    public async Task<ServiceRecordDto> CreateServiceRecord(int carId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken)
    {
        var car = await _context.Cars
            .Include(c => c.SellerNavigation)
            .Include(c => c.StatusNavigation)
            .FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);

        if (car == null)
            throw new KeyNotFoundException("Car not found.");

        var requester = await _context.Users
            .Include(u => u.RoleNavigation)
            .FirstOrDefaultAsync(u => u.Email == requesterEmail, cancellationToken);

        if (requester == null)
            throw new ArgumentException("Requester not found.");

        if (car.SellerNavigation.Id != requester.Id && requester.RoleNavigation.RoleName != "Admin")
            throw new UnauthorizedAccessException("You are not allowed to add service records for this car.");

        var record = new Models.Models.ServiceRecord
        {
            Car = carId,
            MileageAtService = dto.MileageAtService,
            ServiceDate = dto.ServiceDate,
            Grade = dto.Grade
        };

        _context.ServiceRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceRecordDto
        {
            Id = record.Id,
            MileageAtService = record.MileageAtService,
            ServiceDate = record.ServiceDate.ToShortDateString(),
            Grade = record.Grade,
            CarId = record.Car
        };
    }

    public async Task<ServiceRecordDto> UpdateServiceRecord(int carId, int recordId, CreateServiceRecordDto dto, string requesterEmail, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
            .Include(sr => sr.CarNavigation)
                .ThenInclude(c => c.SellerNavigation)
            .FirstOrDefaultAsync(sr => sr.Id == recordId && sr.Car == carId, cancellationToken);

        if (record == null)
            throw new KeyNotFoundException("Service record not found.");

        var requester = await _context.Users
            .Include(u => u.RoleNavigation)
            .FirstOrDefaultAsync(u => u.Email == requesterEmail, cancellationToken);

        if (requester == null)
            throw new ArgumentException("Requester not found.");

        if (record.CarNavigation.SellerNavigation.Id != requester.Id && requester.RoleNavigation.RoleName != "Admin")
            throw new UnauthorizedAccessException("You are not allowed to update this service record.");

        record.MileageAtService = dto.MileageAtService;
        record.ServiceDate = dto.ServiceDate;
        record.Grade = dto.Grade;

        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceRecordDto
        {
            Id = record.Id,
            MileageAtService = record.MileageAtService,
            ServiceDate = record.ServiceDate.ToShortDateString(),
            Grade = record.Grade,
            CarId = record.Car
        };
    }

    public async Task DeleteServiceRecord(int carId, int recordId, string requesterEmail, bool isAdmin, CancellationToken cancellationToken)
    {
        var record = await _context.ServiceRecords
            .Include(sr => sr.CarNavigation)
                .ThenInclude(c => c.SellerNavigation)
            .FirstOrDefaultAsync(sr => sr.Id == recordId && sr.Car == carId, cancellationToken);

        if (record == null)
            throw new KeyNotFoundException("Service record not found.");

        var requester = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == requesterEmail, cancellationToken);

        if (requester == null)
            throw new ArgumentException("Requester not found.");

        if (!isAdmin && record.CarNavigation.SellerNavigation.Id != requester.Id)
            throw new UnauthorizedAccessException("You are not allowed to delete this service record.");

        _context.ServiceRecords.Remove(record);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
