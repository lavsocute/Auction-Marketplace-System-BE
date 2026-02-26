using System.Net;
using System.Text.Json;
using AuctionSys.Application.Common.Exceptions;
using AuctionSys.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await HandleExceptionAsync(context, ex.Message, ex.StatusCode, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Đã xảy ra lỗi hệ thống không xác định.");
            await HandleExceptionAsync(context, "Lỗi hệ thống nội bộ vui lòng thử lại sau!", (int)HttpStatusCode.InternalServerError, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, string message, int statusCode, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<string>.Fail(message, statusCode);
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}
