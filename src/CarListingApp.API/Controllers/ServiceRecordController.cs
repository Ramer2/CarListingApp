using System.Security.Claims;
using CarListingApp.Services.DTOs.ServiceRecord;
using CarListingApp.Services.Services.ServiceRecord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("api/cars/{carId:int}/service-records")]
public class ServiceRecordController : ControllerBase
{
    private readonly IServiceRecordsService _serviceRecordService;

    public ServiceRecordController(IServiceRecordsService serviceRecordService)
    {
        _serviceRecordService = serviceRecordService;
    }

    [HttpGet("")]
    public async Task<IResult> GetAll(int carId, CancellationToken cancellationToken)
    {
        var records = await _serviceRecordService.GetAll(carId, cancellationToken);
        return Results.Ok(records);
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int carId, int id, CancellationToken cancellationToken)
    {
        var record = await _serviceRecordService.GetById(carId, id, cancellationToken);
        return Results.Ok(record);
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPost("")]
    public async Task<IResult> CreateServiceRecord(int carId, [FromBody] CreateServiceRecordDto dto, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        var record = await _serviceRecordService.CreateServiceRecord(carId, dto, email, cancellationToken);
        return Results.Ok(record);
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPut("{id:int}")]
    public async Task<IResult> UpdateServiceRecord(int carId, int id, [FromBody] CreateServiceRecordDto dto, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        var record = await _serviceRecordService.UpdateServiceRecord(carId, id, dto, email, cancellationToken);
        return Results.Ok(record);
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteServiceRecord(int carId, int id, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        await _serviceRecordService.DeleteServiceRecord(carId, id, email, cancellationToken);
        return Results.NoContent();
    }
}
