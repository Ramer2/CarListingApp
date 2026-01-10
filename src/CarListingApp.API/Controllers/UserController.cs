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
        try
        {
            return Results.Ok(await _userService.GetAll(cancellationToken));
        } catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpGet("{id}")]
    public async Task<IResult> GetUserById(int? id, CancellationToken cancellationToken)
    {
        try
        {
            if (User.IsInRole("Admin"))
            {
                return Results.Ok(await _userService.GetUserById(id, cancellationToken));
            }
            else
            {
                var email = User.FindFirst(ClaimTypes.Email); 
                if (email == null)
                    return Results.Problem("Invalid credentials.");
                
                var user = await _userService.GetUserByEmail(email.Value, cancellationToken);
                
                if (user.Id != id)
                    return Results.Forbid();
                
                return Results.Ok(user);
            }
        }
        catch (AccessViolationException)
        {
            return Results.Forbid();
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
    
    [Authorize(Roles = "Admin,User,Dealer")]
    [HttpGet("by-email/{email}")]
    public async Task<IResult> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        try
        {
            if (User.IsInRole("Admin"))
            {
                return Results.Ok(await _userService.GetUserByEmail(email, cancellationToken));
            }
            else
            {
                var tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? User.FindFirst("emailaddress")?.Value;
                if (tokenEmail == null)
                    return Results.Problem("Invalid credentials.");

                return Results.Ok(await _userService.GetUserByEmail(tokenEmail, cancellationToken));
            }
        }
        catch (AccessViolationException)
        {
            return Results.Forbid();
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
    
    [HttpPost("")]
    public async Task<IResult> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
            // only admins can create other admins
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!User.IsInRole("Admin") && createUserDto.RoleName.Equals("Admin"))
                    return Results.Forbid();
            }
            
            var user = await _userService.CreateUser(createUserDto, cancellationToken);
            return Results.Ok(user);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPut("{id}")]
    public async Task<IResult> UpdateUser([FromBody] CreateUserDto createUserDto, int id, CancellationToken cancellationToken)
    {
        try
        {
            if (!User.IsInRole("Admin"))
            {
                var email = User.FindFirst(ClaimTypes.Email); 
                if (email == null)
                    return Results.Problem("Invalid credentials.");
                
                var user = await _userService.GetUserByEmail(email.Value, cancellationToken);
                
                // if different user
                // or tries to change role
                // or tries to change blocked status
                if (user.Id != id 
                    || !user.Role.Equals(createUserDto.RoleName) 
                    || user.IsBlocked != createUserDto.IsBlocked)
                    return Results.Forbid();
            }
            
            return Results.Ok(await _userService.UpdateUser(createUserDto, id, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{id}")]
    public async Task<IResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (!User.IsInRole("Admin"))
            {
                var email = User.FindFirst(ClaimTypes.Email); 
                if (email == null)
                    return Results.Problem("Invalid credentials.");
                
                var user = await _userService.GetUserByEmail(email.Value, cancellationToken);
                
                // if different user
                if (user.Id != id)
                    return Results.Forbid();
            }
            
            await _userService.DeleteUser(id, cancellationToken);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}