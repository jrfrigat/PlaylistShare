using PlaylistShare.Database.Entities;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Shared.Enums;
using PlaylistShare.Shared.Yandex;
using YandexMusic.Endpoints;
using YandexMusic.Models.Albums;
using YandexMusic.Models.Artists;
using YandexMusic.Models.Playlists;
using YandexMusic.Models.Tracks;

namespace PlaylistShare.Api.Services;

/// <summary>
/// Операции с каталогом и плейлистами Яндекс Музыки поверх нового клиента
/// <see cref="YandexMusic.IYandexMusicClient"/>. Доменные модели клиента отображаются в DTO
/// (<c>PlaylistShare.Shared.Yandex</c>), которые отдаются клиентскому приложению.
/// </summary>
public class YandexMusicService
{
    private readonly YandexApiService _yandexApiService;

    private YandexMusic.IYandexMusicClient Client => _yandexApiService.Client;

    public YandexMusicService(YandexApiService yandexApiService)
    {
        _yandexApiService = yandexApiService;
    }

    private async Task AuthorizeIfNot(ApplicationUser user)
    {
        if (_yandexApiService.IsAuthorized)
            return;

        var authResult = await _yandexApiService.AuthAsync(user);
        if (authResult != true)
            throw new Exception("Не удалось авторизоваться в Яндекс.Музыке. Проверьте токен.");
    }

    private async Task<string?> GetAccountUidAsync(CancellationToken cancellationToken)
    {
        var status = await Client.Account.GetStatusAsync(cancellationToken);
        return status?.Account.Uid.ToString();
    }

    public async Task<Playlist?> GetPlaylistAsync(ApplicationUser user, string ownerUid, string kind, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);
        return await Client.Playlists.GetAsync(ownerUid, kind, cancellationToken);
    }

    /// <summary>URL обложки плейлиста Яндекс.Музыки (для backfill сохранённых шеринг-плейлистов).</summary>
    public async Task<string?> GetPlaylistCoverUrlAsync(ApplicationUser user, string ownerUid, string kind, CancellationToken cancellationToken = default)
    {
        var playlist = await GetPlaylistAsync(user, ownerUid, kind, cancellationToken);
        return playlist?.Cover?.GetUrl();
    }

    public async Task<YandexPlaylistData?> GetPlaylistDataAsync(ApplicationUser user, string ownerUid, string kind, CancellationToken cancellationToken = default)
    {
        var playlist = await GetPlaylistAsync(user, ownerUid, kind, cancellationToken);
        return playlist is null ? null : MapToPlaylistData(playlist);
    }

    public async Task<(IReadOnlyList<Playlist>? OwnPlaylists, string? AccountUid)> GetOwnFavoritesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);
        var accountUid = await GetAccountUidAsync(cancellationToken);
        if (string.IsNullOrEmpty(accountUid))
            return (null, null);

        var ownPlaylists = await Client.Playlists.GetByUserAsync(accountUid, cancellationToken);
        return (ownPlaylists, accountUid);
    }

    public async Task<Playlist?> CreatePlaylistAsync(ApplicationUser user, string title, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);
        var accountUid = await GetAccountUidAsync(cancellationToken);
        if (string.IsNullOrEmpty(accountUid))
            return null;

        return await Client.Playlists.CreateAsync(accountUid, title, cancellationToken: cancellationToken);
    }

    public async Task<Playlist?> AddTracksAsync(ApplicationUser user, string ownerUid, string kind, IEnumerable<string> trackIds, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);

        var playlist = await Client.Playlists.GetAsync(ownerUid, kind, cancellationToken);
        if (playlist is null)
            return null;

        var tracks = await Client.Tracks.GetManyAsync(trackIds, cancellationToken);
        if (tracks.Count == 0)
            return null;

        var existing = playlist.Tracks.Select(t => t.Id).ToHashSet();
        var toInsert = tracks.Where(t => !existing.Contains(t.Id)).ToList();
        if (toInsert.Count == 0)
            return playlist;

        // The new client inserts one track at a time by (trackId, albumId) at an index, against the
        // current revision. Append to the end, advancing the revision returned by each insert.
        var revision = playlist.Revision;
        var at = playlist.TrackCount;
        Playlist? updated = playlist;

        foreach (var track in toInsert)
        {
            var albumId = track.Albums.FirstOrDefault()?.Id.ToString() ?? string.Empty;
            updated = await Client.Playlists.InsertTrackAsync(ownerUid, kind, track.Id, albumId, at, revision, cancellationToken);
            if (updated is null)
                return null;

            revision = updated.Revision;
            at++;
        }

        return updated;
    }

    public async Task<Playlist?> RemoveTracksAsync(ApplicationUser user, string ownerUid, string kind, IEnumerable<string> trackIds, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);

        var playlist = await Client.Playlists.GetAsync(ownerUid, kind, cancellationToken);
        if (playlist is null)
            return null;

        var removeSet = trackIds.ToHashSet();

        // The new client removes tracks by index range, not by track object. Resolve the indices of
        // the requested tracks and delete them from the highest index down so earlier deletions do
        // not shift the indices that are still pending.
        var indices = playlist.Tracks
            .Select((t, i) => (t.Id, Index: i))
            .Where(x => removeSet.Contains(x.Id))
            .Select(x => x.Index)
            .OrderByDescending(i => i)
            .ToList();

        if (indices.Count == 0)
            return playlist;

        var revision = playlist.Revision;
        Playlist? updated = playlist;

        foreach (var index in indices)
        {
            updated = await Client.Playlists.DeleteTracksAsync(ownerUid, kind, index, index + 1, revision, cancellationToken);
            if (updated is null)
                return null;

            revision = updated.Revision;
        }

        return updated;
    }

    public async Task<string?> GetTrackFileUrlAsync(ApplicationUser user, string trackId, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);
        return await Client.Tracks.GetDirectLinkAsync(trackId, cancellationToken);
    }

    public async Task<Track?> GetYTrackAsync(ApplicationUser user, string trackId, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);
        return await Client.Tracks.GetAsync(trackId, cancellationToken);
    }

    public async Task<YandexSearchResult> SearchAsync(
        ApplicationUser user,
        string query,
        TrackSearchType? searchType = TrackSearchType.All,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);

        var type = searchType switch
        {
            TrackSearchType.Artist => SearchType.Artist,
            TrackSearchType.Album => SearchType.Album,
            TrackSearchType.Playlist => SearchType.Playlist,
            TrackSearchType.Track => SearchType.Track,
            _ => SearchType.All,
        };

        var searchResult = await Client.Search.SearchAsync(query, type, cancellationToken: cancellationToken);
        if (searchResult is null)
            return new YandexSearchResult();

        return new YandexSearchResult
        {
            Tracks = searchResult.Tracks?.Results.Take(limit).Select(MapTrack).ToList(),
            Playlists = searchResult.Playlists?.Results.Take(limit).Select(MapPlaylist).ToList(),
            Artists = searchResult.Artists?.Results.Take(limit).Select(MapArtist).ToList(),
            Albums = searchResult.Albums?.Results.Take(limit).Select(MapAlbum).ToList(),
        };
    }

    public async Task<YandexSearchResult> SearchMyPlaylists(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);

        var result = new YandexSearchResult();

        var accountUid = await GetAccountUidAsync(cancellationToken);
        if (string.IsNullOrEmpty(accountUid))
            return result;

        var ownPlaylists = await Client.Playlists.GetByUserAsync(accountUid, cancellationToken);
        result.Playlists = ownPlaylists.Select(MapPlaylist).ToList();

        var likedPlaylists = await Client.Library.GetLikedPlaylistsAsync(accountUid, cancellationToken);
        result.LikedPlaylists = likedPlaylists
            .Where(l => l.Playlist is not null)
            .Select(l => MapPlaylist(l.Playlist!))
            .ToList();

        // NOTE: the legacy client exposed a single "personal/auto-generated playlists" call. The new
        // client fetches generated playlists individually by generator id, so PersonalPlaylists is left
        // unset here pending a decision on which generators to surface. Verify against the live API.
        return result;
    }

    public async Task<YandexSearchResult> SearchTracksByIdAsync(
        ApplicationUser user,
        string id,
        TrackSearchType searchType,
        CancellationToken cancellationToken = default)
    {
        await AuthorizeIfNot(user);

        var result = new YandexSearchResult();

        switch (searchType)
        {
            case TrackSearchType.All:
                throw new Exception("Для поиска по ID необходимо указать конкретный тип (трек, альбом, исполнитель или плейлист).");

            case TrackSearchType.Track:
                var track = await Client.Tracks.GetAsync(id, cancellationToken);
                if (track is not null)
                    result.Tracks = [MapTrack(track)];
                break;

            case TrackSearchType.Album:
                var album = await Client.Albums.GetWithTracksAsync(id, cancellationToken);
                result.Tracks = album?.Volumes?.SelectMany(v => v).Select(MapTrack).ToList();
                break;

            case TrackSearchType.Artist:
                var brief = await Client.Artists.GetBriefInfoAsync(id, cancellationToken);
                if (brief is not null)
                {
                    result.Albums = brief.Albums.Select(MapAlbum).ToList();
                    result.Tracks = brief.PopularTracks.Select(MapTrack).ToList();
                }
                break;

            case TrackSearchType.Playlist:
                var playlist = await Client.Playlists.GetByUuidAsync(id, cancellationToken);
                result.Tracks = playlist?.Tracks
                    .Where(t => t.Track is not null)
                    .Select(t => MapTrack(t.Track!))
                    .ToList();
                break;

            case TrackSearchType.MyPlaylists:
                // Не поддерживается: "мои плейлисты" не ищутся по ID - см. SearchMyPlaylists.
                throw new ArgumentOutOfRangeException(nameof(searchType), searchType, "Поиск по ID не поддерживается для типа MyPlaylists.");

            default:
                throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
        }

        return result;
    }

    private static YandexPlaylistData MapToPlaylistData(Playlist playlist) => new()
    {
        Title = playlist.Title,
        Description = playlist.Description ?? string.Empty,
        Tracks = playlist.Tracks
            .Where(t => t.Track is not null)
            .Select(t => MapTrack(t.Track!))
            .ToList(),
    };

    private static YandexTrack MapTrack(Track track) => new()
    {
        TrackId = track.Id,
        Title = track.Title,
        Artists = track.Artists.Select(MapArtist).ToList(),
        CoverUri = track.CoverUri ?? string.Empty,
        DurationMs = track.DurationMs,
    };

    private static YandexArtist MapArtist(Artist artist) => new()
    {
        Id = artist.Id.ToString(),
        Name = artist.Name,
        CoverUrl = artist.Cover.GetUrl(),
        Description = artist.Description?.Text ?? string.Empty,
    };

    private static YandexAlbum MapAlbum(Album album) => new()
    {
        Id = album.Id.ToString(),
        Title = album.Title,
        Artists = album.Artists.Select(MapArtist).ToList(),
        CoverUrl = string.IsNullOrEmpty(album.CoverUri) ? album.Cover.GetUrl() : album.CoverUri,
        Description = null,
    };

    private static YandexPlaylist MapPlaylist(Playlist playlist) => new()
    {
        Uuid = playlist.PlaylistUuid ?? string.Empty,
        Kind = playlist.Kind.ToString(),
        OwnerUid = playlist.Owner.Uid.ToString(),
        Title = playlist.Title,
        Description = playlist.Description,
        CoverUrl = playlist.Cover.GetUrl(),
        TrackCount = playlist.TrackCount,
    };
}
