using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlaylistShare.Database.Entities;

namespace PlaylistShare.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    // Контекст ровно один на все провайдеры: провайдер выбирается опциями (UseConnection), а свой
    // набор миграций каждому даёт отдельная сборка миграций (MigrationsAssembly), а не свой подкласс.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<FavoritePlaylist> FavoritePlaylists => Set<FavoritePlaylist>();

    public DbSet<SharedPlaylist> SharedPlaylists => Set<SharedPlaylist>();
    public DbSet<TrackAdditionLog> TrackAdditionLogs => Set<TrackAdditionLog>();
    public DbSet<TrackRemovalLog> TrackRemovalLogs => Set<TrackRemovalLog>();
    public DbSet<YandexAuthSession> YandexAuthSessions => Set<YandexAuthSession>();

    // Таблицы, ключи, индексы, длины и внешние ключи заданы атрибутами на самих сущностях
    // (Entities/*.cs). Ниже остаётся только то, для чего атрибутов в EF Core не существует:
    //
    // 1. OnDelete(DeleteBehavior.*) - поведение при удалении задаётся ТОЛЬКО через fluent API.
    //    Вызовы HasOne/WithMany здесь нужны лишь чтобы адресовать нужную связь: сама связь и её
    //    внешний ключ уже описаны атрибутами [ForeignKey]/[InverseProperty].
    //    Значения не совпадают с конвенцией (обязательный FK по умолчанию Cascade, необязательный -
    //    ClientSetNull), поэтому убрать их нельзя - схема изменится.
    // 2. HasDefaultValue - значение по умолчанию на стороне БД атрибутом не выражается
    //    ([DefaultValue] из System.ComponentModel в EF Core не читается).
    /// <summary>
    /// Здесь остаётся ТОЛЬКО то, чего нельзя выразить атрибутом на сущности: поведение при удалении
    /// (аналога [DeleteBehavior] в EF Core не существует) и значение по умолчанию (нет и у него).
    /// Всё остальное - таблицы, ключи, индексы, длины, обязательность, внешние ключи и парные
    /// навигации - живёт атрибутами в Entities/. Не переносите это сюда обратно, но и не пытайтесь
    /// "дочистить" оставшееся в атрибуты: их для этого нет.
    /// Связи объявлены атрибутами, поэтому HasForeignKey тут не нужен - хватает HasOne/WithMany,
    /// чтобы дотянуться до нужной связи и задать ей OnDelete.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SharedPlaylist>()
               .HasOne(e => e.Creator)
               .WithMany(u => u.OwnedPlaylists)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrackAdditionLog>(entity =>
        {
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany(sp => sp.TrackAdditionLogs)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.AddedByUser)
                  .WithMany()
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TrackRemovalLog>(entity =>
        {
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany()
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RemovedByUser)
                  .WithMany()
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<FavoritePlaylist>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(u => u.FavoritePlaylists)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany()
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<YandexAuthSession>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany()
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(e => e.IsConfirmed)
                  .HasDefaultValue(false);
        });
    }
}
