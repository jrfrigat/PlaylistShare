using Microsoft.EntityFrameworkCore;
using PlaylistShare.Database;
using PlaylistShare.Database.Entities;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared.Enums;
using Xunit;

namespace PlaylistShare.Api.Tests;

/// <summary>
/// Пакетный GetRemovableTrackIdsAsync - тот же набор правил, что и CanRemoveTrackAsync, только
/// решение принимается сразу для всего списка треков. Главная ценность этих тестов -
/// <see cref="Пакетный_метод_совпадает_с_CanRemoveTrackAsync"/>: две реализации правил обязаны
/// сходиться на каждой ветке RemovePermission и для каждого вида пользователя.
///
/// Устройство стенда - как в SharedPlaylistCanRemoveTrackTests: настоящий TrackAdditionLogService
/// поверх InMemory-провайдера с засеянным журналом (интерфейса и virtual-методов у него нет, подменить
/// заглушкой нельзя), первым аргументом сервиса null - SharedPlaylists пакетный метод не трогает.
/// </summary>
public class SharedPlaylistGetRemovableTrackIdsTests : IDisposable
{
    private static readonly Guid PlaylistId = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid OtherPlaylistId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid CreatorId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ThirdUserId = new("55555555-5555-5555-5555-555555555555");

    private const string OwnSession = "session-own";
    private const string ForeignSession = "session-foreign";

    /// <summary>Добавлен OtherUserId из чужой сессии - свой только по совпадению пользователя.</summary>
    private const string TrackOwnUser = "track-own-user";
    /// <summary>Добавлен анонимом из OwnSession - свой только по совпадению сессии.</summary>
    private const string TrackOwnSession = "track-own-session";
    /// <summary>Добавлен посторонним из посторонней сессии - ничей.</summary>
    private const string TrackForeign = "track-foreign";
    /// <summary>Добавлен анонимом из ЧУЖОЙ сессии - тоже ничей (сторож наивного OR, см. SeedMixedJournal).</summary>
    private const string TrackAnonForeign = "track-anon-foreign";
    /// <summary>Добавлен ЧУЖОЙ учёткой из OwnSession - ничей: сессия чужую учётку не перебивает.</summary>
    private const string TrackForeignUserOwnSession = "track-foreign-user-own-session";
    /// <summary>Трек, которого нет в журнале вовсе.</summary>
    private const string TrackUnlogged = "track-unlogged";

    private static readonly string[] AllTracks =
        [TrackOwnUser, TrackOwnSession, TrackForeign, TrackAnonForeign, TrackForeignUserOwnSession, TrackUnlogged];

    /// <summary>Вид пользователя: xunit не умеет класть Guid? в InlineData, поэтому передаём признак.</summary>
    public enum Actor { Creator, Authorized, Anonymous }

    private static Guid? UserOf(Actor actor) => actor switch
    {
        Actor.Creator => CreatorId,
        Actor.Authorized => OtherUserId,
        _ => null
    };

    private readonly ApplicationDbContext _db;
    private readonly SharedPlaylistService _service;

    /// <summary>
    /// Сервис, у которого журнал построен поверх null-контекста: любое обращение к БД падает с NRE.
    /// Так ветки, отвечающие одинаково для всех треков, доказуемо не ходят в журнал.
    /// </summary>
    private static readonly SharedPlaylistService NoDbService = new(null!, new TrackAdditionLogService(null!));

    /// <summary>Фабрика сессии, которой пользоваться нельзя: получение сессии само стоит запросов.</summary>
    private static Task<string> NoSession(CancellationToken _) =>
        throw new InvalidOperationException("сессия не должна запрашиваться на этой ветке");

    private static Task<string> OwnSessionFactory(CancellationToken _) => Task.FromResult(OwnSession);

    public SharedPlaylistGetRemovableTrackIdsTests()
    {
        _db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"removable-track-ids-{Guid.NewGuid()}")
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

    private void SeedAddition(string trackId, Guid? addedByUserId, string sessionId, Guid? playlistId = null)
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

    /// <summary>
    /// Журнал со смесью владельцев: каждый случай предиката разведён по своему треку. TrackOwnUser
    /// ловится только совпадением пользователя, TrackOwnSession - только совпадением сессии, а два
    /// трека не должны достаться никому, и каждый сторожит свою ошибку. TrackAnonForeign - сторож
    /// наивного OR: у анонима "AddedByUserId == userId" выродилось бы в "== null" и подцепило бы его.
    /// TrackForeignUserOwnSession - сторож общего компьютера: сессия та же, но добавлен он из-под
    /// чужой учётки, и засчитывать совпадение сессии тут нельзя ни авторизованному, ни анониму.
    /// </summary>
    private void SeedMixedJournal()
    {
        SeedAddition(TrackOwnUser, addedByUserId: OtherUserId, sessionId: ForeignSession);
        SeedAddition(TrackOwnSession, addedByUserId: null, sessionId: OwnSession);
        SeedAddition(TrackForeign, addedByUserId: ThirdUserId, sessionId: ForeignSession);
        SeedAddition(TrackAnonForeign, addedByUserId: null, sessionId: ForeignSession);
        SeedAddition(TrackForeignUserOwnSession, addedByUserId: ThirdUserId, sessionId: OwnSession);
        // TrackUnlogged не засеваем намеренно.
    }

    private Task<HashSet<string>> GetRemovable(
        SharedPlaylist playlist,
        Guid? userId,
        IReadOnlyCollection<string>? trackIds = null,
        Func<CancellationToken, Task<string>>? sessionIdFactory = null) =>
        _service.GetRemovableTrackIdsAsync(playlist, userId, trackIds ?? AllTracks, sessionIdFactory ?? OwnSessionFactory);

    /// <summary>Сравнение по составу с читаемым отчётом о расхождении.</summary>
    private static void AssertSameSet(IEnumerable<string> expected, IEnumerable<string> actual) =>
        Assert.Equal(
            expected.OrderBy(x => x, StringComparer.Ordinal),
            actual.OrderBy(x => x, StringComparer.Ordinal));

    // ---------- Главное: совпадение с CanRemoveTrackAsync ----------

    /// <summary>
    /// Пакетный метод обязан давать ровно то же, что поштучный обход тех же треков через
    /// CanRemoveTrackAsync - на всех ветках RemovePermission и для создателя / авторизованного /
    /// анонима. Ожидание считается вызовом самого CanRemoveTrackAsync, а не выписано руками: так тест
    /// ловит расхождение реализаций, даже если однажды поменяются сами правила.
    /// </summary>
    [Theory]
    [InlineData(EditPermission.Everyone, Actor.Creator)]
    [InlineData(EditPermission.Everyone, Actor.Authorized)]
    [InlineData(EditPermission.Everyone, Actor.Anonymous)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Creator)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Authorized)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Anonymous)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Creator)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Authorized)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Anonymous)]
    [InlineData(EditPermission.Nobody, Actor.Creator)]
    [InlineData(EditPermission.Nobody, Actor.Authorized)]
    [InlineData(EditPermission.Nobody, Actor.Anonymous)]
    public async Task Пакетный_метод_совпадает_с_CanRemoveTrackAsync(EditPermission permission, Actor actor)
    {
        SeedMixedJournal();
        var playlist = NewPlaylist(permission);
        var userId = UserOf(actor);

        var expected = new List<string>();
        foreach (var trackId in AllTracks)
        {
            if (await _service.CanRemoveTrackAsync(playlist, userId, trackId, OwnSession))
                expected.Add(trackId);
        }

        AssertSameSet(expected, await GetRemovable(playlist, userId));
    }

    // ---------- Ветки, одинаковые для всех треков, не ходят в БД ----------

    /// <summary>
    /// Ради этого пакетный метод и заведён: там, где ответ одинаков для всех треков, показ страницы
    /// не должен стоить ни одного запроса. Сервис с null-контекстом и запрещённая фабрика сессии
    /// делают любое обращение к БД падением теста.
    /// </summary>
    [Theory]
    [InlineData(EditPermission.Everyone, Actor.Creator)]
    [InlineData(EditPermission.Everyone, Actor.Authorized)]
    [InlineData(EditPermission.Everyone, Actor.Anonymous)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Authorized)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Anonymous)]
    [InlineData(EditPermission.Nobody, Actor.Authorized)]
    [InlineData(EditPermission.Nobody, Actor.Anonymous)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Creator)]
    public async Task Одинаковые_для_всех_треков_ветки_не_трогают_ни_журнал_ни_сессию(EditPermission permission, Actor actor)
    {
        var result = await NoDbService.GetRemovableTrackIdsAsync(
            NewPlaylist(permission), UserOf(actor), AllTracks, NoSession);

        // Ответ всё равно должен быть осмысленным: либо все треки, либо ни одного.
        Assert.True(result.Count == 0 || result.Count == AllTracks.Length);
    }

    // ---------- Раздача признака внутри AddedByUserOnly ----------

    /// <summary>
    /// Авторизованному засчитываются свои добавления по учётке и АНОНИМНЫЕ добавления из его сессии.
    /// TrackForeignUserOwnSession в набор не попадает: сессия та же, но учётка чужая.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_авторизованный_получает_свои_треки_и_анонимные_из_своей_сессии()
    {
        SeedMixedJournal();

        AssertSameSet([TrackOwnUser, TrackOwnSession], await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    /// <summary>
    /// Аноним получает только анонимные добавления своей сессии. TrackAnonForeign анонимный, но из
    /// чужой сессии; TrackForeignUserOwnSession из его сессии, но под чужой учёткой - оба мимо.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_аноним_получает_только_анонимные_треки_своей_сессии()
    {
        SeedMixedJournal();

        AssertSameSet([TrackOwnSession], await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), null));
    }

    [Fact]
    public async Task AddedByUserOnly_создатель_получает_все_треки_независимо_от_журнала()
    {
        SeedMixedJournal();

        AssertSameSet(AllTracks, await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), CreatorId));
    }

    // ---------- Область поиска в журнале ----------

    /// <summary>Журнал плейлиста шире его нынешнего состава: возвращаем только спрошенные треки.</summary>
    [Fact]
    public async Task AddedByUserOnly_в_ответ_не_попадают_треки_которых_нет_в_запросе()
    {
        SeedAddition("track-removed-from-playlist", addedByUserId: OtherUserId, sessionId: OwnSession);
        SeedAddition(TrackOwnUser, addedByUserId: OtherUserId, sessionId: OwnSession);

        AssertSameSet([TrackOwnUser], await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    [Fact]
    public async Task AddedByUserOnly_журнал_чужого_плейлиста_не_даёт_права()
    {
        SeedAddition(TrackOwnUser, addedByUserId: OtherUserId, sessionId: OwnSession, playlistId: OtherPlaylistId);

        Assert.Empty(await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    /// <summary>
    /// Один трек могли добавить дважды (удалили и вернули): Distinct в журнальном запросе не должен
    /// превращаться в дубли, а признак остаётся признаком.
    /// </summary>
    [Fact]
    public async Task AddedByUserOnly_повторное_добавление_трека_не_ломает_набор()
    {
        SeedAddition(TrackOwnUser, addedByUserId: OtherUserId, sessionId: OwnSession);
        SeedAddition(TrackOwnUser, addedByUserId: OtherUserId, sessionId: ForeignSession);

        AssertSameSet([TrackOwnUser], await GetRemovable(NewPlaylist(EditPermission.AddedByUserOnly), OtherUserId));
    }

    [Fact]
    public async Task Пустой_плейлист_даёт_пустой_набор()
    {
        Assert.Empty(await GetRemovable(NewPlaylist(EditPermission.Everyone), CreatorId, trackIds: []));
    }
}
