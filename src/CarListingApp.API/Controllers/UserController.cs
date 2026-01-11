using System.Security.Claims;
using CarListingApp.Services.DTOs.User;
using CarListingApp.Services.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        return Results.Ok(await _userService.GetAll(cancellationToken));
    }

    [Authorize(Roles = "Admin,User,Dealer")]
    [HttpGet("{id}")]
    public async Task<IResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var requesterEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (requesterEmail == null)
            return Results.Unauthorized();

        return Results.Ok(await _userService.GetUserById(id, requesterEmail, cancellationToken));
    }
    
    [Authorize(Roles = "Admin,User,Dealer")]
    [HttpGet("by-email/{email}")]
    public async Task<IResult> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var requesterEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (requesterEmail == null)
            return Results.Unauthorized();

        return Results.Ok(await _userService.GetUserByEmail(email, requesterEmail, cancellationToken));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("")]
    public async Task<IResult> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var requesterEmail = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.Email)
            : null;

        return Results.Ok(await _userService.CreateUser(createUserDto, requesterEmail, cancellationToken));
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPut("{id}")]
    public async Task<IResult> UpdateUser([FromBody] CreateUserDto createUserDto, int id, CancellationToken cancellationToken)
    {
        var requesterEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (requesterEmail == null)
            return Results.Unauthorized();
            
        return Results.Ok(await _userService.UpdateUser(createUserDto, id, requesterEmail, cancellationToken));
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{id}")]
    public async Task<IResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var requesterEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (requesterEmail == null)
            return Results.Unauthorized();
            
        await _userService.DeleteUser(id, requesterEmail, cancellationToken);
        return Results.NoContent();
    }
}