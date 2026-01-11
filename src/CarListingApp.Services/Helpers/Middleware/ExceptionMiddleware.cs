using CarListingApp.Services.Exceptions.Auth;
using CarListingApp.Services.Exceptions.Car;
using CarListingApp.Services.Exceptions.Favorite;
using CarListingApp.Services.Exceptions.Record;
using CarListingApp.Services.Exceptions.User;
using CarListingApp.Services.Exceptions.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace CarListingApp.Services.Helpers.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            // Auth
            case AuthenticationFailedException:
                statusCode = StatusCodes.Status401Unauthorized;
                message = exception.Message;
                break;
            case AuthorizationFailedException:
            case UserBlockedException:
                statusCode = StatusCodes.Status403Forbidden;
                message = exception.Message;
                break;

            // Car
            case CarNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
                break;
            case VinDuplicateException:
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
                break;

            // Favorite
            case CarAlreadyFavoritedException:
            case FavoriteNotFoundException:
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
                break;

            // User
            case UserAlreadyExistsException:
            case UserSellLimitException:
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
                break;
            case UserNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
                break;

            // Record
            case RecordNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
                break;

            // Validation
            case InvalidIdException:
            case InvalidPasswordException:
            case InvalidEmailException:
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var errorObj = new { error = message };
        var json = JsonSerializer.Serialize(errorObj);

        var bytes = Encoding.UTF8.GetBytes(json);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
    }
}