using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Entities;

namespace PlaylistShare.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Lets provider-specific subclasses (e.g. PostgresDbContext) pass their own typed options through.
    protected ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<FavoritePlaylist> FavoritePlaylists => Set<FavoritePlaylist>();

    public DbSet<SharedPlaylist> SharedPlaylists => Set<SharedPlaylist>();
    public DbSet<TrackAdditionLog> TrackAdditionLogs => Set<TrackAdditionLog>();
    public DbSet<TrackRemovalLog> TrackRemovalLogs => Set<TrackRemovalLog>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<YandexAuthSession> YandexAuthSessions => Set<YandexAuthSession>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SharedPlaylist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShareToken).IsUnique();
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.OwnedPlaylists)
                  .HasForeignKey(e => e.CreatorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.YandexPlaylistUuid).IsRequired().HasMaxLength(50);
            entity.Property(e => e.YandexPlaylistKind).IsRequired().HasMaxLength(50);
            entity.Property(e => e.YandexPlaylistOwnerUid).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
        });

        builder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).HasMaxLength(449);
            entity.HasIndex(e => e.AssociatedUserId);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.AssociatedUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<TrackAdditionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SharedPlaylistId, e.TrackId });
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany(sp => sp.TrackAdditionLogs)
                  .HasForeignKey(e => e.SharedPlaylistId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.AddedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.AddedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Session)
                  .WithMany(s => s.TrackAdditionLogs)
                  .HasForeignKey(e => e.SessionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TrackRemovalLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SharedPlaylistId, e.TrackId });
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany()
                  .HasForeignKey(e => e.SharedPlaylistId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RemovedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.RemovedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Session)
                  .WithMany(s => s.TrackRemovalLogs)
                  .HasForeignKey(e => e.SessionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(e => e.RefreshToken)
                  .HasDatabaseName("IX_AspNetUsers_RefreshToken");
        });

        builder.Entity<FavoritePlaylist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SharedPlaylistId });
            entity.HasOne(e => e.User)
                  .WithMany(u => u.FavoritePlaylists)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SharedPlaylist)
                  .WithMany()
                  .HasForeignKey(e => e.SharedPlaylistId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.AddedAtUtc).IsRequired();
        });


        builder.Entity<YandexAuthSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(e => e.QrCodeUrl)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.ConfirmedAt)
                .IsRequired(false);
            entity.Property(e => e.IsConfirmed)
                .IsRequired()
                .HasDefaultValue(false);
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_YandexAuthSessions_UserId");
        });
    }
}