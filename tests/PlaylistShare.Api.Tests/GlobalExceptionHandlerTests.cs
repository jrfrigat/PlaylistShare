using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlaylistShare.Api.Extensions;
using Xunit;

namespace PlaylistShare.Api.Tests;

/// <summary>
/// Обработчик проверяется напрямую через DefaultHttpContext: поднимать хост не нужно, вся его
/// работа - это код ответа и тело.
/// </summary>
public class GlobalExceptionHandlerTests
{
    private const string TraceId = "trace-42";

    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "PlaylistShare.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    /// <summary>Запоминает записи, чтобы сверить traceId в логе с traceId в теле ответа.</summary>
    private sealed class RecordingLogger : ILogger<GlobalExceptionHandler>
    {
        public List<(LogLevel Level, string Message, Exception? Exception)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception), exception));
    }

    private sealed class Harness
    {
        public required GlobalExceptionHandler Handler { get; init; }
        public required DefaultHttpContext Context { get; init; }
        public required MemoryStream Body { get; init; }
        public required RecordingLogger Logger { get; init; }

        public string BodyAsString()
        {
            Body.Position = 0;
            return new StreamReader(Body).ReadToEnd();
        }

        public JsonDocument BodyAsJson() => JsonDocument.Parse(BodyAsString());
    }

    private static Harness NewHarness(string? environmentName = null)
    {
        var logger = new RecordingLogger();
        var context = new DefaultHttpContext { TraceIdentifier = TraceId };
        context.Request.Method = "POST";
        context.Request.Path = "/api/sharedplaylist/abc/add-tracks";
        var body = new MemoryStream();
        context.Response.Body = body;

        return new Harness
        {
            Handler = new GlobalExceptionHandler(
                logger,
                new FakeHostEnvironment { EnvironmentName = environmentName ?? Environments.Production }),
            Context = context,
            Body = body,
            Logger = logger,
        };
    }

    /// <summary>Возвращает значение свойства или null, если его нет или оно null.</summary>
    private static string? ReadOptionalString(JsonDocument document, string propertyName) =>
        document.RootElement.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null
            ? value.GetString()
            : null;

    // ---------- Обычное исключение ----------

    [Fact]
    public async Task Необработанное_исключение_превращается_в_500_с_телом_ProblemDetails()
    {
        var harness = NewHarness();

        var handled = await harness.Handler.TryHandleAsync(
            harness.Context, new InvalidOperationException("сломалось"), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, harness.Context.Response.StatusCode);

        using var json = harness.BodyAsJson();
        Assert.Equal(500, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Внутренняя ошибка сервера", json.RootElement.GetProperty("title").GetString());
        Assert.Equal("POST /api/sharedplaylist/abc/add-tracks", json.RootElement.GetProperty("instance").GetString());
    }

    [Fact]
    public async Task Тело_ответа_содержит_traceId_запроса()
    {
        var harness = NewHarness();

        await harness.Handler.TryHandleAsync(harness.Context, new Exception("сломалось"), CancellationToken.None);

        using var json = harness.BodyAsJson();
        Assert.Equal(TraceId, json.RootElement.GetProperty("traceId").GetString());
    }

    /// <summary>
    /// Смысл traceId в том, чтобы по жалобе пользователя найти запись в логе. Если значения
    /// разойдутся, идея развалится, а обычный тест на 500 этого не заметит.
    /// </summary>
    [Fact]
    public async Task Лог_и_тело_ответа_содержат_один_и_тот_же_traceId()
    {
        var harness = NewHarness();
        var exception = new InvalidOperationException("сломалось");

        await harness.Handler.TryHandleAsync(harness.Context, exception, CancellationToken.None);

        var entry = Assert.Single(harness.Logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Contains(TraceId, entry.Message);

        using var json = harness.BodyAsJson();
        var traceIdFromBody = json.RootElement.GetProperty("traceId").GetString();
        Assert.NotNull(traceIdFromBody);
        Assert.Contains(traceIdFromBody, entry.Message);
    }

    [Fact]
    public async Task Ответ_отдаётся_как_json()
    {
        var harness = NewHarness();

        await harness.Handler.TryHandleAsync(harness.Context, new Exception("сломалось"), CancellationToken.None);

        Assert.StartsWith("application/json", harness.Context.Response.ContentType);
    }

    // ---------- Detail по окружениям ----------

    [Fact]
    public async Task В_Production_детали_исключения_клиенту_не_уходят()
    {
        var harness = NewHarness(Environments.Production);

        await harness.Handler.TryHandleAsync(
            harness.Context, new InvalidOperationException("секретный путь C:/keys/private.pem"), CancellationToken.None);

        using var json = harness.BodyAsJson();
        Assert.Null(ReadOptionalString(json, "detail"));
        Assert.DoesNotContain("private.pem", harness.BodyAsString());
        Assert.DoesNotContain("InvalidOperationException", harness.BodyAsString());
    }

    [Fact]
    public async Task В_Development_детали_исключения_попадают_в_тело()
    {
        var harness = NewHarness(Environments.Development);

        await harness.Handler.TryHandleAsync(
            harness.Context, new InvalidOperationException("сломалось"), CancellationToken.None);

        using var json = harness.BodyAsJson();
        var detail = ReadOptionalString(json, "detail");
        Assert.NotNull(detail);
        Assert.Contains("InvalidOperationException", detail);
        Assert.Contains("сломалось", detail);
    }

    [Fact]
    public async Task В_Staging_детали_исключения_клиенту_не_уходят()
    {
        var harness = NewHarness(Environments.Staging);

        await harness.Handler.TryHandleAsync(
            harness.Context, new InvalidOperationException("сломалось"), CancellationToken.None);

        using var json = harness.BodyAsJson();
        Assert.Null(ReadOptionalString(json, "detail"));
    }

    // ---------- Отменённый запрос ----------

    [Fact]
    public async Task Отменённый_запрос_отдаёт_499_без_тела()
    {
        var harness = NewHarness();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var handled = await harness.Handler.TryHandleAsync(
            harness.Context, new OperationCanceledException(cts.Token), cts.Token);

        Assert.True(handled);
        Assert.Equal(499, harness.Context.Response.StatusCode);
        Assert.Equal(0, harness.Body.Length);
        Assert.Null(harness.Context.Response.ContentType);
    }

    [Fact]
    public async Task Отменённый_запрос_не_пишется_в_лог()
    {
        var harness = NewHarness();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await harness.Handler.TryHandleAsync(harness.Context, new OperationCanceledException(cts.Token), cts.Token);

        Assert.Empty(harness.Logger.Entries);
    }

    [Fact]
    public async Task TaskCanceledException_при_отменённом_токене_тоже_даёт_499()
    {
        var harness = NewHarness();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await harness.Handler.TryHandleAsync(harness.Context, new TaskCanceledException(), cts.Token);

        Assert.Equal(499, harness.Context.Response.StatusCode);
        Assert.Equal(0, harness.Body.Length);
    }

    /// <summary>
    /// 499 оправдан только тем, что ушёл клиент. Если токен не сработал, отмена пришла изнутри
    /// (таймаут, сорванная фоновая операция) - это настоящая ошибка, её надо залогировать и вернуть 500.
    /// </summary>
    [Fact]
    public async Task Отмена_без_сработавшего_токена_остаётся_ошибкой_500()
    {
        var harness = NewHarness();

        await harness.Handler.TryHandleAsync(
            harness.Context, new OperationCanceledException("таймаут внутри сервиса"), CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, harness.Context.Response.StatusCode);
        Assert.NotEqual(0, harness.Body.Length);
        Assert.Single(harness.Logger.Entries);
    }
}
