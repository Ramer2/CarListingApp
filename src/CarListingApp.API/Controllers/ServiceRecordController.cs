using System.Security.Claims;
using CarListingApp.Services.DTOs.ServiceRecord;
using CarListingApp.Services.Services.ServiceRecord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("cars/{carId:int}/service-records")]
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
        try
        {
            var records = await _serviceRecordService.GetAll(carId, cancellationToken);
            return Results.Ok(records);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetById(int carId, int id, CancellationToken cancellationToken)
    {
        try
        {
            var record = await _serviceRecordService.GetById(carId, id, cancellationToken);
            return Results.Ok(record);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPost("")]
    public async Task<IResult> CreateServiceRecord(int carId, [FromBody] CreateServiceRecordDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Results.Unauthorized();

            var record = await _serviceRecordService.CreateServiceRecord(carId, dto, email, cancellationToken);
            return Results.Ok(record);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPut("{id:int}")]
    public async Task<IResult> UpdateServiceRecord(int carId, int id, [FromBody] CreateServiceRecordDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Results.Unauthorized();

            var record = await _serviceRecordService.UpdateServiceRecord(carId, id, dto, email, cancellationToken);
            return Results.Ok(record);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteServiceRecord(int carId, int id, CancellationToken cancellationToken)
    {
        try
        {
            var isAdmin = User.IsInRole("Admin");
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!isAdmin && email == null)
                return Results.Unauthorized();

            await _serviceRecordService.DeleteServiceRecord(carId, id, email, isAdmin, cancellationToken);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
