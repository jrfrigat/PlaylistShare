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
    /// Общий компьютер: пользователь A залогинен и добавил трек, потом в том же браузере разлогинился
    /// и зашёл B. Серверная сессия у браузера одна на всех, кто в нём побывал, поэтому совпадение
    /// сессии тут есть - но трек добавлен из-под учётки A, и снять его может только A. Сессия чужую
    /// учётку не перебивает: она лишь замена учётки для анонима.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_совпадение_сессии_не_даёт_авторизованному_трек_чужой_учётки()
    {
        SeedAddition(addedByUserId: ThirdUserId, sessionId: OwnSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    /// <summary>
    /// Обратная сторона того же правила и причина, по которой сессия вообще участвует: аноним добавил
    /// трек (в журнале AddedByUserId=null, SessionId=его), затем залогинился в том же браузере.
    /// Совпадения по AddedByUserId нет, но добавление анонимное и сессия та же - трек его.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_залогинившийся_удаляет_трек_добавленный_им_же_анонимно()
    {
        SeedAddition(addedByUserId: null, sessionId: OwnSession);

        Assert.True(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    /// <summary>
    /// Сторож наивного OR: если предикат упростить до "AddedByUserId == userId || SessionId ==
    /// sessionId", то у анонима (userId == null) первое сравнение станет "AddedByUserId == null" и
    /// совпадёт с ЛЮБЫМ анонимным добавлением - вот с этим. Тест обязан падать при таком упрощении.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_аноним_не_удаляет_трек_добавленный_анонимом_из_другой_сессии()
    {
        SeedAddition(addedByUserId: null, sessionId: ForeignSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    /// <summary>Совпадение по пользователю не зависит от сессии: свой трек виден из любого браузера.</summary>
    [Fact]
    public async Task AddedByUserOnly_авторизованный_удаляет_свой_трек_из_другой_сессии()
    {
        SeedAddition(addedByUserId: OtherUserId, sessionId: ForeignSession);

        Assert.True(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
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
    /// Та же дыра, мягче: залогиненный добавил трек и разлогинился в том же браузере. Сессия совпала,
    /// но добавление сделано из-под учётки, а у анонима её нет - предъявить ему нечего. Учётка сильнее
    /// сессии, поэтому разлогин отбирает право снять свой же трек, и это правильный размен.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_аноним_не_удаляет_трек_добавленный_из_его_сессии_залогиненным()
    {
        SeedAddition(addedByUserId: ThirdUserId, sessionId: OwnSession);

        Assert.False(await CanRemove(NewPlaylist(EditPermission.AddedByUserOnly), null));
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
