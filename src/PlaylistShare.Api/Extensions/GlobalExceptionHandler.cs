using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistShare.Api.Extensions;

/// <summary>
/// Ловит всё, что не поймали контроллеры. До него необработанное исключение уходило клиенту голым
/// 500 без тела, а в логах не оставалось ничего - искать причину приходилось вслепую.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Отменённый запрос - не ошибка: клиент ушёл со страницы или закрыл вкладку. Отвечать
        // некому, а в логи такое сыпать незачем. 499 - код nginx для "клиент разорвал соединение".
        if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = 499;
            return true;
        }

        // TraceIdentifier отдаём клиенту и пишем в лог одним и тем же значением: по нему конкретную
        // жалобу пользователя можно найти в логах, не выясняя время и обстоятельства.
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception on {Method} {Path} (traceId {TraceId})",
            httpContext.Request.Method, httpContext.Request.Path, traceId);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Внутренняя ошибка сервера",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Extensions = { ["traceId"] = traceId },
        };

        // Detail только в разработке: текст исключения выдаёт устройство системы, пути и куски
        // запросов - в проде это подсказка атакующему. В проде остаётся traceId и лог на сервере.
        if (_environment.IsDevelopment())
            problem.Detail = exception.ToString();

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
