using System.Text;
using CarListingApp.DAL.DBContext;
using CarListingApp.Services.Helpers.JwtOptions;
using CarListingApp.Services.Services.CarService;
using CarListingApp.Services.Services.TokenService;
using CarListingApp.Services.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();