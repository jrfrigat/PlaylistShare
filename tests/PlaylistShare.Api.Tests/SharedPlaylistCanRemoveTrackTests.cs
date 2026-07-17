using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared.Enums;
using Xunit;

namespace PlaylistShare.Api.Tests;

/// <summary>
/// Ветки switch в CanRemoveTrackAsync. Ветка AddedByUserOnly ходит в журнал добавлений, поэтому
/// журнал здесь настоящий, но поверх InMemory-провайдера с заранее засеянными строками.
///
/// Ручную заглушку подставить не получается: TrackAdditionLogService - обычный класс без интерфейса
/// и без virtual-методов, а SharedPlaylistService принимает его по конкретному типу. Наследник с
/// new-методом не помог бы - вызов из сервиса привязан к типу поля и ушёл бы в базовую реализацию.
/// Засеянный InMemory-журнал даёт ту же управляемость и заодно проверяет реальный текст запроса.
///
/// Первым аргументом сервиса идёт null: удаление не должно трогать SharedPlaylists напрямую, всё
/// решение принимается по плейлисту из аргумента и журналу.
/// </summary>
public class SharedPlaylistCanRemoveTrackTests : IDisposable
{
    private static readonly Guid PlaylistId = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid OtherPlaylistId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid CreatorId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ThirdUserId = new("55555555-5555-5555-5555-555555555555");

    private const string TrackId = "track-1";
    private const string OwnSession = "session-own";
    private const string ForeignSession = "session-foreign";

    private readonly ApplicationDbContext _db;
    private readonly SharedPlaylistService _service;

    public SharedPlaylistCanRemoveTrackTests()
    {
        _db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"remove-track-{Guid.NewGuid()}")
            .Options);
        _service = new SharedPlaylistService(null!, new TrackAdditionLogService(_db));
    }

    public void Dispose() => _db.Dispose();

    private SharedPlaylist NewPlaylist(EditPermission remove) => new()
    {
        Id = PlaylistId,
        CreatorUserId = CreatorId,
        YandexPlaylistUuid = "uuid",
        YandexPlaylistKind = "1000",
        YandexPlaylistOwnerUid = "owner",
        Title = "Плейлист",
        ShareToken = "token",
        RemovePermission = remove,
    };

    private void SeedAddition(Guid? addedByUserId, string sessionId, string trackId = TrackId, Guid? playlistId = null)
    {
        _db.TrackAdditionLogs.Add(new TrackAdditionLog
        {
            Id = Guid.NewGuid(),
            SharedPlaylistId = playlistId ?? PlaylistId,
            TrackId = trackId,
            AddedByUserId = addedByUserId,
            AddedAtUtc = DateTime.UtcNow,
            SessionId = sessionId,
        });
        _db.SaveChanges();
    }

    private Task<bool> CanRemove(SharedPlaylist playlist, Guid? userId, string sessionId = OwnSession) =>
        _service.CanRemoveTrackAsync(playlist, userId, TrackId, sessionId);

    // ---------- Создатель ----------

    [Theory]
    [InlineData(EditPermission.Everyone)]
    [InlineData(EditPermission.AuthorizedOnly)]
    [InlineData(EditPermission.Nobody)]
    [InlineData(EditPermission.AddedByUserOnly)]
    public async Task Создатель_удаляет_любой_трек_при_любом_праве(EditPermission permission)
    {
        // Журнал пуст: создатель не должен зависеть от того, кто добавил трек.
        Assert.True(await CanRemove(NewPlaylist(permission), CreatorId));
    }

    // ---------- Everyone / AuthorizedOnly / Nobody ----------

    [Theory]
    [InlineData(EditPermission.Everyone, true)]
    [InlineData(EditPermission.AuthorizedOnly, true)]
    [InlineData(EditPermission.Nobody, false)]
    public async Task Авторизованный_чужой_удаляет_согласно_праву(EditPermission permission, bool expected)
    {
        Assert.Equal(expected, await CanRemove(NewPlaylist(permission), OtherUserId));
    }

    [Theory]
    [InlineData(EditPermission.Everyone, true)]
    [InlineData(EditPermission.AuthorizedOnly, false)]
    [InlineData(EditPermission.Nobody, false)]
    public async Task Аноним_удаляет_согласно_праву(EditPermission permission, bool expected)
    {
        Assert.Equal(expected, await CanRemove(NewPlaylist(permission), null));
    }

    // ---------- AddedByUserOnly, авторизованный ----------

    [Fact]
    public async Task AddedByUserOnly_авторизованный_удаляет_свой_трек()
    {
        SeedAddition(addedByUserId: OtherUserId, sessionId: OwnSession);

        Assert.True(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    [Fact]
    public async Task AddedByUserOnly_авторизованный_не_удаляет_чужой_трек()
    {
        SeedAddition(addedByUserId: ThirdUserId, sessionId: ForeignSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    [Fact]
    public async Task AddedByUserOnly_авторизованный_не_удаляет_трек_которого_нет_в_журнале()
    {
        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    /// <summary>
    /// Для авторизованного сверяется только AddedByUserId, sessionId в расчёт не идёт. Тем самым
    /// чужую строку журнала нельзя присвоить, попав в ту же сессию.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_совпадение_сессии_не_даёт_авторизованному_чужой_трек()
    {
        SeedAddition(addedByUserId: ThirdUserId, sessionId: OwnSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    // ---------- AddedByUserOnly, аноним по sessionId ----------

    [Fact]
    public async Task AddedByUserOnly_аноним_удаляет_трек_добавленный_своей_сессией()
    {
        SeedAddition(addedByUserId: null, sessionId: OwnSession);

        Assert.True(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    [Fact]
    public async Task AddedByUserOnly_аноним_не_удаляет_трек_добавленный_чужой_сессией()
    {
        SeedAddition(addedByUserId: null, sessionId: ForeignSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    [Fact]
    public async Task AddedByUserOnly_аноним_не_удаляет_трек_которого_нет_в_журнале()
    {
        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    /// <summary>
    /// Аноним сверяется только по SessionId, поэтому строка, добавленная залогиненным пользователем
    /// из той же сессии, для анонима всё равно "своя". Фиксируем как есть: сессия одна и та же.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_аноним_удаляет_трек_добавленный_из_его_сессии_залогиненным()
    {
        SeedAddition(addedByUserId: ThirdUserId, sessionId: OwnSession);

        Assert.True(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    // ---------- Область поиска в журнале ----------

    [Fact]
    public async Task AddedByUserOnly_журнал_чужого_плейлиста_не_даёт_права()
    {
        SeedAddition(addedByUserId: OtherUserId, sessionId: OwnSession, playlistId: OtherPlaylistId);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    [Fact]
    public async Task AddedByUserOnly_запись_о_другом_треке_не_даёт_права()
    {
        SeedAddition(addedByUserId: OtherUserId, sessionId: OwnSession, trackId: "track-2");

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }
}
