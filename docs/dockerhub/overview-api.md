# PlaylistShare API

ASP.NET Core Web API for **PlaylistShare** - share and collaborate on Yandex Music playlists. This image
is the backend host: it authenticates users, talks to Yandex Music, and stores the shared playlists,
collaboration logs and favorites. Built on **.NET 10**. It pairs with the `playlistshare-pwa` front end.

The database is selectable at runtime via `Database__Provider`: **SQL Server** (default) or **PostgreSQL**.
Pending EF Core migrations are applied automatically on startup, so a fresh deployment self-provisions its
schema. It connects to a SQL Server / PostgreSQL that you provide; it does not bundle a database.

## What's inside

- ASP.NET Core API host, published framework-dependent on `mcr.microsoft.com/dotnet/aspnet:10.0`.
- Runs as a non-root user and listens on container port **8080** (the .NET base-image default).
- EF Core with two providers (SQL Server / Npgsql), JWT auth, and ASP.NET Data Protection whose key ring
  lives on a mounted volume (not in the database).

## Run

```sh
docker run -p 7001:8080 \
  -e ConnectionStrings__DefaultConnection="Server=...;Database=PlaylistShare;User Id=...;Password=...;TrustServerCertificate=True" \
  -e Jwt__Key="a-long-stable-secret" \
  -e Database__Provider=SqlServer \
  -v playlistshare-keys:/keys -e DataProtection__KeysPath=/keys \
  frigat/playlistshare-api:latest
# API on http://localhost:7001
```

Environment variables:

- `ConnectionStrings__DefaultConnection` (required) - connection string for the chosen provider.
- `Jwt__Key` (required) - stable secret used to sign auth tokens.
- `Database__Provider` (optional) - `SqlServer` (default) or `Postgres`. For Postgres, supply a matching
  Npgsql connection string.
- `DataProtection__KeysPath` (optional) - path to persist the Data Protection key ring; mount a volume
  here so keys survive restarts. Defaults to `/keys`.

For a full stack (API + PWA + optional containerized Postgres) use the Compose files in the repository.

## Tags

- `latest` - the most recent release.
- `<version>` (e.g. `1.0.0`) - a pinned version (recommended for production).

## Links

- Source, full documentation and issues: https://github.com/jrfrigat/PlaylistShare
- Front end: `playlistshare-pwa`
- Also published to GHCR: `ghcr.io/jrfrigat/playlistshare-api`
- License: MIT
