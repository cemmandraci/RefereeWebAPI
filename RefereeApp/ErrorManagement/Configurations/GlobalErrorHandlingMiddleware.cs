using System.Net;
using RefereeApp.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;
using KeyNotFoundException = RefereeApp.Exceptions.KeyNotFoundException;
using NotImplementedException = RefereeApp.Exceptions.NotImplementedException;
using UnauthorizedAccessException = RefereeApp.Exceptions.UnauthorizedAccessException;

namespace RefereeApp.ErrorManagement.Configurations;

//TODO : stackTrace 'e bakılacak.
//TODO : Dönüşler kontrol edilecek.

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string stackTrace;
        string message;

        var exceptionType = exception.GetType();

        if (exceptionType == typeof(BadRequestException))
        {
            message = exception.Message;
            status = HttpStatusCode.BadRequest;
            stackTrace = exception.StackTrace;
        }
        else if (exceptionType == typeof(NotFoundException))
        {
            message = exception.Message;
            status = HttpStatusCode.NotFound;
            stackTrace = exception.StackTrace;
        }
        else if (exceptionType == typeof(NotImplementedException))
        {
            message = exception.Message;
            status = HttpStatusCode.NotImplemented;
            stackTrace = exception.StackTrace;
        }
        else if (exceptionType == typeof(UnauthorizedAccessException))
        {
            message = exception.Message;
            status = HttpStatusCode.Unauthorized;
            stackTrace = exception.StackTrace;
        }
        else if (exceptionType == typeof(KeyNotFoundException))
        {
            message = exception.Message;
            status = HttpStatusCode.Unauthorized;
            stackTrace = exception.StackTrace;
        }
        else
        {
            status = HttpStatusCode.InternalServerError;
            message = exception.Message;
            stackTrace = exception.StackTrace;
        }

        var exceptionResult = JsonSerializer.Serialize(new { error = status, message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(exceptionResult);
    }
    
}