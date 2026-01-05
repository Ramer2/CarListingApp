using CarListingApp.Services.DTOs;
using CarListingApp.Services.DTOs.User;
using CarListingApp.Services.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("users")]
public class UserController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
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

    [HttpGet]
    [Route("{id}")]
    public async Task<IResult> GetUserById(int? id, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await _userService.GetUserById(id, cancellationToken));
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
    public async Task<IResult> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
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

    [HttpPut]
    [Route("{id}")]
    public async Task<IResult> UpdateUser([FromBody] CreateUserDto createUserDto, int id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.UpdateUser(createUserDto, id, cancellationToken);
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

    [HttpDelete]
    [Route("{id}")]
    public async Task<IResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        try
        {
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