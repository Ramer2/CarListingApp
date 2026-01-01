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
            return Results.Ok( await _userService.GetUserById(id, cancellationToken));
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