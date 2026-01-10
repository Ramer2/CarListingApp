using System.Security.Claims;
using CarListingApp.Services.DTOs.Car;
using CarListingApp.Services.Services.CarService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("api/cars")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }
    
    [HttpGet("")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        try
        { 
            return Results.Ok(await _carService.GetAll(cancellationToken));
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await _carService.GetById(id, cancellationToken));
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
    public async Task<IResult> CreateCar([FromBody] CreateCarDto createCarDto, CancellationToken cancellationToken)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Results.Unauthorized();
                
            return Results.Ok(await _carService.CreateCar(createCarDto, email, cancellationToken));
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
    [HttpPut("{id}")]
    public async Task<IResult> UpdateCar([FromBody] CreateCarDto createCarDto, int id, CancellationToken cancellationToken)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Results.Unauthorized();
                
            return Results.Ok(await _carService.UpdateCar(createCarDto, id, email, cancellationToken));
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
    [HttpDelete("{id}")]
    public async Task<IResult> DeleteCar(int id, CancellationToken cancellationToken)
    {
        try
        {
            var isAdmin = User.IsInRole("Admin");

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!isAdmin && email == null)
                return Results.Unauthorized();

            await _carService.DeleteCar(
                id,
                email,
                isAdmin,
                cancellationToken);

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