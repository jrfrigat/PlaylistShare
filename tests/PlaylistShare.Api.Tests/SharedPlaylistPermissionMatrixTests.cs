using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared.Enums;
using Xunit;

namespace PlaylistShare.Api.Tests;

/// <summary>
/// Полная матрица прав для синхронных проверок CanView / CanPlay / CanAddTrack.
///
/// Сервис здесь намеренно строится с null-зависимостями: если какая-то из этих проверок однажды
/// начнёт ходить в базу или в журнал добавлений, тест упадёт с NullReferenceException. Это дешёвый
/// сторож на то, что проверки прав остаются чистыми вычислениями в памяти.
/// </summary>
public class SharedPlaylistPermissionMatrixTests
{
    private static readonly Guid CreatorId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = new("22222222-2222-2222-2222-222222222222");

    /// <summary>Кто именно спрашивает права: владелец, посторонний с логином или аноним.</summary>
    public enum Actor
    {
        Creator,
        OtherUser,
        Anonymous,
    }

    private static Guid? UserIdOf(Actor actor) => actor switch
    {
        Actor.Creator => CreatorId,
        Actor.OtherUser => OtherUserId,
        Actor.Anonymous => null,
        _ => throw new ArgumentOutOfRangeException(nameof(actor)),
    };

    private static SharedPlaylistService NewService() => new(null!, null!);

    private static SharedPlaylist NewPlaylist(
        ViewPermission view = ViewPermission.Everyone,
        ViewPermission play = ViewPermission.Everyone,
        EditPermission add = EditPermission.Everyone,
        EditPermission remove = EditPermission.Everyone) => new()
        {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            CreatorUserId = CreatorId,
            YandexPlaylistUuid = "uuid",
            YandexPlaylistKind = "1000",
            YandexPlaylistOwnerUid = "owner",
            Title = "Плейлист",
            ShareToken = "token",
            ViewPermission = view,
            PlayPermission = play,
            AddPermission = add,
            RemovePermission = remove,
        };

    // ---------- CanView: ViewPermission x 3 актора ----------

    [Theory]
    [InlineData(ViewPermission.Everyone, Actor.Creator, true)]
    [InlineData(ViewPermission.Everyone, Actor.OtherUser, true)]
    [InlineData(ViewPermission.Everyone, Actor.Anonymous, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.Creator, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.OtherUser, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.Anonymous, false)]
    public void Просмотр_разрешён_согласно_матрице(ViewPermission permission, Actor actor, bool expected)
    {
        var playlist = NewPlaylist(view: permission);

        var actual = NewService().CanView(playlist, UserIdOf(actor));

        Assert.Equal(expected, actual);
    }

    // ---------- CanPlay: PlayPermission x 3 актора ----------

    [Theory]
    [InlineData(ViewPermission.Everyone, Actor.Creator, true)]
    [InlineData(ViewPermission.Everyone, Actor.OtherUser, true)]
    [InlineData(ViewPermission.Everyone, Actor.Anonymous, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.Creator, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.OtherUser, true)]
    [InlineData(ViewPermission.AuthorizedOnly, Actor.Anonymous, false)]
    public void Прослушивание_разрешено_согласно_матрице(ViewPermission permission, Actor actor, bool expected)
    {
        var playlist = NewPlaylist(play: permission);

        var actual = NewService().CanPlay(playlist, UserIdOf(actor));

        Assert.Equal(expected, actual);
    }

    // ---------- CanAddTrack: EditPermission x 3 актора ----------

    [Theory]
    [InlineData(EditPermission.Everyone, Actor.Creator, true)]
    [InlineData(EditPermission.Everyone, Actor.OtherUser, true)]
    [InlineData(EditPermission.Everyone, Actor.Anonymous, true)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Creator, true)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.OtherUser, true)]
    [InlineData(EditPermission.AuthorizedOnly, Actor.Anonymous, false)]
    [InlineData(EditPermission.Nobody, Actor.Creator, true)]
    [InlineData(EditPermission.Nobody, Actor.OtherUser, false)]
    [InlineData(EditPermission.Nobody, Actor.Anonymous, false)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Creator, true)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.OtherUser, false)]
    [InlineData(EditPermission.AddedByUserOnly, Actor.Anonymous, false)]
    public void Добавление_трека_разрешено_согласно_матрице(EditPermission permission, Actor actor, bool expected)
    {
        var playlist = NewPlaylist(add: permission);

        var actual = NewService().CanAddTrack(playlist, UserIdOf(actor));

        Assert.Equal(expected, actual);
    }

    // ---------- Отдельно зафиксированные гарантии ----------

    [Fact]
    public void Создатель_проходит_любую_проверку_при_самых_закрытых_правах()
    {
        var playlist = NewPlaylist(
            view: ViewPermission.AuthorizedOnly,
            play: ViewPermission.AuthorizedOnly,
            add: EditPermission.Nobody,
            remove: EditPermission.Nobody);
        var service = NewService();

        Assert.True(service.CanView(playlist, CreatorId));
        Assert.True(service.CanPlay(playlist, CreatorId));
        Assert.True(service.CanAddTrack(playlist, CreatorId));
    }

    [Theory]
    [InlineData(EditPermission.Nobody)]
    [InlineData(EditPermission.AddedByUserOnly)]
    public void Добавлять_треки_при_Nobody_и_AddedByUserOnly_может_только_создатель(EditPermission permission)
    {
        var playlist = NewPlaylist(add: permission);
        var service = NewService();

        Assert.True(service.CanAddTrack(playlist, CreatorId));
        Assert.False(service.CanAddTrack(playlist, OtherUserId));
        Assert.False(service.CanAddTrack(playlist, null));
    }

    [Fact]
    public void Аноним_не_проходит_ни_одну_проверку_с_AuthorizedOnly()
    {
        var playlist = NewPlaylist(
            view: ViewPermission.AuthorizedOnly,
            play: ViewPermission.AuthorizedOnly,
            add: EditPermission.AuthorizedOnly);
        var service = NewService();

        Assert.False(service.CanView(playlist, null));
        Assert.False(service.CanPlay(playlist, null));
        Assert.False(service.CanAddTrack(playlist, null));
    }

    /// <summary>
    /// Права независимы: закрытый на просмотр плейлист не должен неявно закрывать прослушивание
    /// и наоборот. Тест фиксирует текущее поведение, чтобы случайная "оптимизация" одной проверки
    /// через другую не прошла молча.
    /// </summary>
    [Fact]
    public void Права_на_просмотр_и_прослушивание_не_влияют_друг_на_друга()
    {
        var service = NewService();

        var viewClosed = NewPlaylist(view: ViewPermission.AuthorizedOnly, play: ViewPermission.Everyone);
        Assert.False(service.CanView(viewClosed, null));
        Assert.True(service.CanPlay(viewClosed, null));

        var playClosed = NewPlaylist(view: ViewPermission.Everyone, play: ViewPermission.AuthorizedOnly);
        Assert.True(service.CanView(playClosed, null));
        Assert.False(service.CanPlay(playClosed, null));
    }

    /// <summary>
    /// CanPlayEveryone используется в AudioController для анонимной отдачи потока и намеренно
    /// не смотрит на пользователя вообще - даже создатель через него не пройдёт.
    /// </summary>
    [Theory]
    [InlineData(ViewPermission.Everyone, true)]
    [InlineData(ViewPermission.AuthorizedOnly, false)]
    public void CanPlayEveryone_смотрит_только_на_право_и_игнорирует_пользователя(ViewPermission permission, bool expected)
    {
        var playlist = NewPlaylist(play: permission);

        Assert.Equal(expected, NewService().CanPlayEveryone(playlist));
    }

    /// <summary>
    /// currentUserId сравнивается с CreatorUserId как Guid? с Guid: null никогда не совпадает.
    /// Отдельный тест на случай, если сравнение однажды перепишут через Equals или GetValueOrDefault -
    /// тогда аноним стал бы "создателем" плейлиста с CreatorUserId = Guid.Empty.
    /// </summary>
    [Fact]
    public void Аноним_не_считается_создателем_плейлиста_с_пустым_CreatorUserId()
    {
        var playlist = NewPlaylist(view: ViewPermission.AuthorizedOnly, add: EditPermission.Nobody);
        playlist.CreatorUserId = Guid.Empty;
        var service = NewService();

        Assert.False(service.CanView(playlist, null));
        Assert.False(service.CanAddTrack(playlist, null));
    }
}
