<p align="center">
  <img src="assets/banner.png" alt="PlaylistShare - share Yandex Music playlists" width="860">
</p>

# PlaylistShare

<p align="center">🌐 <b>English</b> - <a href="README.ru.md">Русский</a></p>

[![CI](https://github.com/jrfrigat/PlaylistShare/actions/workflows/ci.yml/badge.svg)](https://github.com/jrfrigat/PlaylistShare/actions/workflows/ci.yml)
[![CodeQL](https://github.com/jrfrigat/PlaylistShare/actions/workflows/codeql.yml/badge.svg)](https://github.com/jrfrigat/PlaylistShare/actions/workflows/codeql.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com)
[![Docker Pulls](https://img.shields.io/docker/pulls/frigat/playlistshare-api?logo=docker&label=Docker%20pulls)](https://hub.docker.com/r/frigat/playlistshare-api)

**PlaylistShare** lets you share **Yandex Music** playlists with people who do not have access to
them, and collaborate on a shared track list. It is a small full-stack **.NET 10** app: an ASP.NET
Core API and a Blazor WebAssembly progressive web app, packaged as two Docker images.

> **Unofficial project. Not affiliated with Yandex.** It talks to Yandex Music on a user's behalf
> via their own credentials - use at your own risk and comply with the service's terms.

---

## Features

- **Share a playlist via a link** with per-share permissions (view / play / add / remove)
- **Collaborate on the track list** - add and remove tracks together, with an audit log of who changed what
- **Search the Yandex catalogue** (tracks, albums, artists, playlists)
- **In-browser playback** of tracks via direct stream links
- **Favorites** plus your liked and own playlists
- **Link your Yandex Music account** by QR sign-in or by pasting an access token
- **Installable PWA** with a dynamic-color purple theme

---

## Architecture

Three projects, one solution (`PlaylistShare.slnx`):

| Project | Description |
|---------|-------------|
| `src/PlaylistShare.Api` | ASP.NET Core API - JWT auth + ASP.NET Core Identity, EF Core (SQL Server **or** PostgreSQL), Yandex Music integration, OpenAPI. |
| `src/PlaylistShare.Pwa` | Blazor WebAssembly client built on [Flare](https://github.com/jrfrigat/Flare), served as a PWA behind nginx. |
| `src/PlaylistShare.Shared` | DTOs/contracts shared between the API and the client. |

Yandex Music calls are powered by the [YandexMusic](https://github.com/jrfrigat/YandexMusic) client.
Users sign in locally with a username and password (JWT), then link a Yandex Music account via QR
sign-in or by pasting an access token.

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- `dotnet workload install wasm-tools` (Blazor WebAssembly build tools)
- SQL Server or PostgreSQL (or use Docker - see below)

### Build & run

```sh
dotnet restore PlaylistShare.slnx
dotnet build   PlaylistShare.slnx -c Release

# API - OpenAPI document at /openapi/v1.json
dotnet run --project src/PlaylistShare.Api

# PWA client
dotnet run --project src/PlaylistShare.Pwa
```

The API applies pending EF Core migrations automatically on startup (`Database.Migrate()`), so there
is no manual `dotnet ef database update` step - a fresh database self-provisions on first run.

---

## Docker

The API and PWA ship as two images, published on Docker Hub and the GitHub Container Registry:

| Image | Docker Hub | GHCR |
|-------|------------|------|
| API (ASP.NET Core) | `frigat/playlistshare-api` | `ghcr.io/jrfrigat/playlistshare-api` |
| PWA (Blazor WASM, nginx) | `frigat/playlistshare-pwa` | `ghcr.io/jrfrigat/playlistshare-pwa` |

### Full stack with docker-compose

The quickest way to run everything - API, PWA and a bundled PostgreSQL - is a small `docker-compose.yml`.
Copy it, change the two secrets (`Jwt__Key` and the database password), then `docker compose up -d`:

```yaml
services:
  db:
    image: postgres:17-alpine
    environment:
      POSTGRES_DB: PlaylistShare
      POSTGRES_USER: playlist
      POSTGRES_PASSWORD: change-me
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U playlist -d PlaylistShare"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    image: frigat/playlistshare-api:latest
    depends_on:
      db:
        condition: service_healthy
    environment:
      Database__Provider: Postgres
      ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=PlaylistShare;Username=playlist;Password=change-me"
      Jwt__Key: "change-me-to-a-stable-secret-at-least-32-characters"
      DataProtection__KeysPath: /keys
    volumes:
      - dpkeys:/keys
    ports:
      - "7001:8080"

  pwa:
    image: frigat/playlistshare-pwa:latest
    depends_on:
      - api
    ports:
      - "7101:80"

volumes:
  pgdata:
  dpkeys:
```

```sh
docker compose up -d
# PWA:  http://localhost:7101
# API:  http://localhost:7001   (OpenAPI document at /openapi/v1.json)
```

The API applies its EF Core migrations on startup, so the database provisions itself on first run. To
use SQL Server instead of the bundled PostgreSQL, drop the `db` service, set `Database__Provider=SqlServer`
and point `ConnectionStrings__DefaultConnection` at your MS SQL Server.

> Note: the PWA calls the API at the address baked into its `wwwroot/appsettings.json` (`ApiBaseUrl`),
> which the published image sets to the project's own domain. To host the PWA under your own domain,
> serve the API on the same origin behind a reverse proxy (and add that origin to `Cors:Origins`), or
> rebuild the PWA image from source with your own `ApiBaseUrl`.

### API only

The API image is self-contained and configured entirely through environment variables (it listens on
container port `8080`):

```sh
docker run -p 7001:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=PlaylistShare;User Id=sa;Password=...;TrustServerCertificate=True" \
  -e Jwt__Key="change-me-to-a-stable-secret-at-least-32-characters" \
  -e Database__Provider=SqlServer \
  -v playlistshare-keys:/keys -e DataProtection__KeysPath=/keys \
  frigat/playlistshare-api:latest
# API on http://localhost:7001
```

### From source

For development, or to build both images with your own configuration, use the compose files in this
repository. `run-docker-compose.bat` layers `docker-compose.prod.yml` (secrets from a git-ignored `.env`
plus a persistent Data Protection key volume) over the base `docker-compose.yml`:

```sh
cp .env.example .env      # then edit .env
run-docker-compose.bat    # docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

Database options for the source build: **SQL Server** (default; point `CONNECTION_STRING` at an external
MS SQL Server), an **external PostgreSQL** (`DB_PROVIDER=Postgres` plus a matching Npgsql
`CONNECTION_STRING`), or a **containerized PostgreSQL** via `run-docker-compose-postgres.bat`. See
`.env.example` for every variable.

---

## Configuration

`src/PlaylistShare.Api/appsettings.json` ships with **placeholders only**. Provide real values
through [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) in development,
or environment variables in production (the double-underscore form is shown in parentheses):

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | Database connection string for the active provider. |
| `Jwt:Key` | Signing key for issued JWTs (>= 32 chars). |
| `Database:Provider` (`Database__Provider`) | `SqlServer` (default) or `Postgres`. |
| `DataProtection:KeysPath` (`DataProtection__KeysPath`) | Directory for the Data Protection key ring - a mounted volume in Docker, **not** the database. |
| `Cors:Origins` | Allowed origins for the PWA. |
| `Client:BaseUrl` | Public base URL of the PWA, used to build share links. |

```sh
cd src/PlaylistShare.Api
dotnet user-secrets set "Jwt:Key" "..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

**Never commit real secrets.** See [SECURITY.md](SECURITY.md).

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Bug reports and feature requests go through the
[issue templates](https://github.com/jrfrigat/PlaylistShare/issues/new/choose).

---

## License

MIT (c) 2026 FrigaT
