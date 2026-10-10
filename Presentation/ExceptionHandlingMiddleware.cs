using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace EventServiceApi.Presentation.Middleware;

/// <summary>
/// Middleware для глобальной обработки необработанных исключений.
/// Формирует единообразный JSON-ответ в формате Problem Details (RFC 7807).
/// 
/// Соответствие HTTP-статусов:
/// - 400 Bad Request       — ошибки валидации / некорректные аргументы;
/// - 404 Not Found         — ресурс не найден;
/// - 500 Internal Server Error — все непредвиденные ошибки.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Обработка HTTP-запроса с перехватом исключений
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Если ответ уже начал отправляться — прерываем обработку
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "Ответ уже начал отправляться, невозможно обработать исключение {ExceptionType}",
                exception.GetType().Name);
            return;
        }

        var (statusCode, title, detail) = MapException(exception);

        // Логируем: Warning для ожидаемых ошибок (400/404), Error для 500
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Необработанное исключение при обработке запроса {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "Ожидаемая ошибка {StatusCode} при обработке запроса {Method} {Path}: {Message}",
                statusCode,
                context.Request.Method,
                context.Request.Path,
                exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // Добавляем traceId для диагностики
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Сопоставление типа исключения с HTTP-статусом, заголовком и деталью.
    /// Возвращает безопасный для клиента текст (без внутренних деталей для 500).
    /// </summary>
    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) => exception switch
    {
        ArgumentNullException or ArgumentException
            => (StatusCodes.Status400BadRequest,
                "Некорректные аргументы запроса",
                exception.Message),

        KeyNotFoundException
            => (StatusCodes.Status404NotFound,
                "Ресурс не найден",
                exception.Message),

        // Всё остальное — непредвиденная ошибка → 500.
        // Детали исключения НЕ отдаём клиенту (только в лог).
        _ => (StatusCodes.Status500InternalServerError,
              "Внутренняя ошибка сервера",
              "Произошла непредвиденная ошибка. Обратитесь в поддержку, указав traceId.")
    };
}