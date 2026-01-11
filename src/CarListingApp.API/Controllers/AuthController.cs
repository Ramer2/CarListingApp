using CarListingApp.Services.DTOs.Auth;
using CarListingApp.Services.DTOs.User;
using CarListingApp.Services.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginUserDto userDto, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await _authService.Login(userDto, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (AccessViolationException ex)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.Register(createUserDto, cancellationToken);
            return Results.Created();
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (AccessViolationException ex)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}