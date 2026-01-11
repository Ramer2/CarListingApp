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
        return Results.Ok(await _carService.GetAll(cancellationToken));
    }
    
    [HttpGet("{id}")]
    public async Task<IResult> GetById(int id, CancellationToken cancellationToken)
    {
        return Results.Ok(await _carService.GetById(id, cancellationToken));
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPost("")]
    public async Task<IResult> CreateCar([FromBody] CreateCarDto createCarDto, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();
                
        return Results.Ok(await _carService.CreateCar(createCarDto, email, cancellationToken));
    }
    
    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPut("{id}")]
    public async Task<IResult> UpdateCar([FromBody] CreateCarDto createCarDto, int id, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();
                
        return Results.Ok(await _carService.UpdateCar(createCarDto, id, email, cancellationToken));
    }
    
    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{id}")]
    public async Task<IResult> DeleteCar(int id, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        await _carService.DeleteCar(id, email, cancellationToken);

        return Results.NoContent();
    }
}