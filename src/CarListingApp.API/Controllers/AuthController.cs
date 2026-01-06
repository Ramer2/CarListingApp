using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Services.DTOs.Auth;
using CarListingApp.Services.Services.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.API.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User>  _passwordHasher = new();
    private readonly CarListingContext _context;

    public AuthController(ITokenService tokenService, CarListingContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IResult> Login([FromBody] LoginUserDto userDto, CancellationToken cancellationToken)
    {
        try
        {
            if (userDto.Email == null && userDto.Username == null)
                return Results.BadRequest("Email or username is required.");

            User? user;
            
            if (userDto.Email != null)
            {
                user = await _context.Users
                    .Include(u => u.RoleNavigation)
                    .FirstOrDefaultAsync(u => u.Email.Equals(userDto.Email), cancellationToken);
            }
            else
            {
                user = await _context.Users
                    .Include(u => u.RoleNavigation)
                    .FirstOrDefaultAsync(u => u.Username.Equals(userDto.Username), cancellationToken);
            }
            
            if (user == null)
                return Results.Unauthorized();

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            if (verification == PasswordVerificationResult.Failed)
                return Results.Unauthorized();

            var token = new TokenDto
            {
                AccessToken = _tokenService.GenerateToken(
                    user.Username,
                    user.RoleNavigation.RoleName,
                    user.Email)
            };
            
            return Results.Ok(token);
        } catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}