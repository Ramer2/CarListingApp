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
        return Results.Ok(await _authService.Login(userDto, cancellationToken));
    }

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        await _authService.Register(createUserDto, cancellationToken);
        return Results.Created();
    }
}