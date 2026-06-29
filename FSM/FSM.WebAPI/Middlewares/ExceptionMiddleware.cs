using System.Net;
using System.Text.Json;

namespace FSM.WebAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    // _next: "Benden sonraki adıma (yani Controller'a) geçebilirsin" iznini temsil eder.
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // İsteği normal akışına bırakıyoruz. Garsona (Controller'a) gitmesini söylüyoruz.
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Kodun NERESİNDE patlama olursa olsun (Service, Repo, DB), akış buraya düşer!
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Bizim fırlattığımız iş kuralları (throw new Exception) genelde 400 Bad Request'tir.
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = exception.Message, // Bizim yazdığımız "Teknisyen müsait değil" yazısı
            sentAt = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}