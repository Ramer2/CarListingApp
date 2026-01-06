using System.Security.Claims;
using CarListingApp.Services.DTOs.Car;
using CarListingApp.Services.Services.CarService;
using CarListingApp.Services.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("cars")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IUserService _userService;

    public CarController(ICarService carService, IUserService userService)
    {
        _carService = carService;
        _userService = userService;
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
            if (User.IsInRole("Admin"))
            { 
                if (createCarDto.SellerId == -1)
                    return Results.BadRequest("Insufficient seller details. No id provided.");
                
                var seller = await _userService.GetUserById(createCarDto.SellerId, cancellationToken);
                return Results.Ok(await _carService.CreateCar(createCarDto, seller.Email, cancellationToken));
            }
            else
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                    return Results.Problem("Invalid credentials. No email provided.");
                
                return Results.Ok(await _carService.CreateCar(createCarDto, email, cancellationToken));
            }
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