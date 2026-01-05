using CarListingApp.Services.DTOs.Car;
using CarListingApp.Services.Services.CarService;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("cars")]
public class CarController
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
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

    [HttpGet]
    [Route("{id}")]
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

    [HttpPost]
    public async Task<IResult> CreateCar([FromBody] CreateCarDto createCarDto, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await _carService.CreateCar(createCarDto, cancellationToken));
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
}