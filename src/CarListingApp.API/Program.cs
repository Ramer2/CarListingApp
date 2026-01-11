using System.Text;
using CarListingApp.DAL.DBContext;
using CarListingApp.Services.Helpers.JwtOptions;
using CarListingApp.Services.Helpers.Middleware;
using CarListingApp.Services.Services.Auth;
using CarListingApp.Services.Services.CarService;
using CarListingApp.Services.Services.Favorite;
using CarListingApp.Services.Services.ServiceRecord;
using CarListingApp.Services.Services.TokenService;
using CarListingApp.Services.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5004")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
    });
});

var jwtConfigData = builder.Configuration.GetSection("Jwt");

var connectionString = builder.Configuration.GetConnectionString("CarListingDB");
builder.Services.AddDbContext<CarListingContext>(options => options.UseSqlite(connectionString));

builder.Services.Configure<JwtOptions>(jwtConfigData);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtConfigData["Issuer"],
        ValidAudience = jwtConfigData["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigData["Key"])),
        ClockSkew = TimeSpan.FromMinutes(10)
    };
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IServiceRecordsService, ServiceRecordsService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<BlockedUserMiddleware>();

app.MapControllers();

app.Run();