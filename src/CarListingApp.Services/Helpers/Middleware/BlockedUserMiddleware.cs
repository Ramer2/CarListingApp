using System.Security.Claims;
using CarListingApp.DAL.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.Services.Helpers.Middleware;

public class BlockedUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CarListingContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                var isBlocked = await dbContext.Users
                    .Where(u => u.Email == email)
                    .Select(u => u.IsBlocked)
                    .FirstOrDefaultAsync(context.RequestAborted);

                if (isBlocked)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        await _next(context);
    }
}
