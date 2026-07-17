using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Services;
using PlaylistShare.Database;
using PlaylistShare.Database.Entities;
using PlaylistShare.Shared.SharedPlaylist;
using Xunit;

namespace PlaylistShare.Api.Tests;

/// <summary>
/// Название плейлиста приходит из Яндекса, а не от клиента, и может быть длиннее колонки
/// nvarchar(255). CreateAsync обязан его обрезать - иначе SaveChanges падает 500 в проде.
/// InMemory длину сам не проверяет, поэтому тест проверяет именно обрезку, а не отказ БД.
/// </summary>
public class SharedPlaylistCreateTitleTests
{
    private static SharedPlaylistService NewService(out ApplicationDbContext db)
    {
        db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"create-title-{Guid.NewGuid()}")
            .Options);
        return new SharedPlaylistService(db, new TrackAdditionLogService(db));
    }

    private static SharePlaylistDto DtoWithTitle(string title) => new()
    {
        YandexPlaylistUuid = "uuid",
        YandexPlaylistKind = "kind",
        YandexPlaylistOwnerUid = "owner",
        Title = title,
    };

    [Fact]
    public async Task Слишком_длинное_название_обрезается_до_лимита_колонки()
    {
        var service = NewService(out var db);
        var longTitle = new string('a', SharedPlaylist.TitleMaxLength + 50);

        var result = await service.CreateAsync(Guid.NewGuid(), DtoWithTitle(longTitle));

        var stored = await db.SharedPlaylists.SingleAsync();
        Assert.Equal(SharedPlaylist.TitleMaxLength, stored.Title.Length);
        Assert.Equal(longTitle[..SharedPlaylist.TitleMaxLength], stored.Title);
        // DTO на выходе несёт то же обрезанное значение, а не исходное.
        Assert.Equal(SharedPlaylist.TitleMaxLength, result.Title.Length);
    }

    [Fact]
    public async Task Название_в_пределах_лимита_не_меняется()
    {
        var service = NewService(out var db);
        var title = new string('a', SharedPlaylist.TitleMaxLength);

        await service.CreateAsync(Guid.NewGuid(), DtoWithTitle(title));

        var stored = await db.SharedPlaylists.SingleAsync();
        Assert.Equal(title, stored.Title);
    }
}
