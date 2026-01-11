using System.Security.Claims;
using CarListingApp.Services.Services.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarListingApp.API.Controllers;

[ApiController]
[Route("api/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesService _favoritesService;

    public FavoritesController(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpPost("")]
    public async Task<IResult> AddToFavorites([FromBody] int carId, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        await _favoritesService.AddToFavorites(carId, email, cancellationToken);
        return Results.Created();
    }
    
    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpDelete("{carId}")]
    public async Task<IResult> RemoveFromFavorites(int carId, CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        await _favoritesService.RemoveFromFavorites(carId, email, cancellationToken);
        return Results.NoContent();
    }

    [Authorize(Roles = "Admin, User, Dealer")]
    [HttpGet("")]
    public async Task<IResult> GetFavorites(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Results.Unauthorized();

        return Results.Ok(await _favoritesService.GetFavorites(email, cancellationToken));
    }
}