using CarListingApp.DAL.DBContext;
using CarListingApp.Models.Models;
using CarListingApp.Models.Models.Enums;
using CarListingApp.Services.DTOs.Auth;
using CarListingApp.Services.DTOs.User;
using CarListingApp.Services.Services.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Services.Auth;

public class AuthService : IAuthService
{
    private readonly CarListingContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(CarListingContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<TokenDto> Login(LoginUserDto loginUserDto, CancellationToken cancellationToken)
    {
        if (loginUserDto.Email == null && loginUserDto.Username == null)
            throw new ArgumentException("Email or username is required.");

        User? user;
            
        if (loginUserDto.Email != null)
        {
            user = await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(u => u.Email.Equals(loginUserDto.Email), cancellationToken);
        }
        else
        {
            user = await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(u => u.Username.Equals(loginUserDto.Username), cancellationToken);
        }

        if (user == null || user.IsBlocked)
            throw new AccessViolationException();

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
        if (verification == PasswordVerificationResult.Failed)
            throw new AccessViolationException();

        var token = new TokenDto
        {
            AccessToken = _tokenService.GenerateToken(
                user.Username,
                user.RoleNavigation.RoleName,
                user.Email)
        };
            
        return token;
    }

    public async Task Register(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        if (!(createUserDto.Role == RolesEnum.Dealer || createUserDto.Role == RolesEnum.User))
            throw new AccessViolationException("You can register only a user or a dealer.");
        
        if (string.IsNullOrEmpty(createUserDto.Password))
            throw new ArgumentException("Password must be valid.");
        
        if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email, cancellationToken))
            throw new ArgumentException("An account with this email already exists.");
        if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username, cancellationToken))
            throw new ArgumentException("This username is already taken.");

        var newUser = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            CreatedAt = DateTime.Now,
            IsBlocked = false,
            Role = (int) createUserDto.Role,
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, createUserDto.Password);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);
    }
}