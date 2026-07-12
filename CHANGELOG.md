# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2026-07-13]

### Added

- Selectable database provider: **SQL Server (default) or PostgreSQL**, chosen via `Database:Provider`
  (env `Database__Provider`; `SqlServer` | `Postgres`). Each provider carries its own EF Core
  migration set, and the session cache lives in the chosen database. Two ways to run Postgres in
  Docker: an external instance (`DB_PROVIDER=Postgres` + a Postgres `CONNECTION_STRING` in `.env`) or a
  containerized one via `docker-compose.postgres.yml` (`run-docker-compose-postgres.bat`).

### Changed

- Rebranded the product to **PlaylistShare** and rewrote the README (EN + RU) and changelog to match
  the current architecture, database options, and Docker workflow.
- Backend optimization pass across `PlaylistShare.Api` (startup, DI, and data-access cleanup).
- Moved the Data Protection key ring out of the database onto a **mounted volume** (`DataProtection:KeysPath`,
  `/keys` in Docker), so keys survive redeploys independently of the database.
- Externalized production secrets to a git-ignored `.env` file, applied through the
  `docker-compose.prod.yml` overlay (see `.env.example`).

### Removed

- Unused **Keycloak / OpenID** integration and the **Yandex OAuth** client credentials
  (`Yandex:ClientId` / `Yandex:ClientSecret`). Yandex Music accounts are linked via QR sign-in or by
  pasting an access token; there is no "Sign in with Yandex" OAuth flow.

## [Unreleased]

### Changed

- Restructured the repository to a single-root layout (solution and tooling at the root, projects
  under `src/`), mirroring the sibling Flare repository.
- Migrated the Yandex Music integration from the legacy private `YandexMusic.API` package to the new
  [YandexMusic](https://github.com/jrfrigat/YandexMusic) client.
- Migrated the PWA UI from MudBlazor to [Flare](https://github.com/jrfrigat/Flare) (`Flare.Blazor`).
- Replaced the generic Material Design 3 theme with a bespoke "Deka" Flare theme (own OKLCH palette,
  Onest typography, rounder shape scale) ported from the Deka Playlist Share design, and restyled every
  PWA page/component to match it; the PWA now references `Flare.Theme.MaterialDesign3Expressive` (for its
  design-token baseline) instead of `Flare.Theme.MaterialDesign3`.
- Updated all dependencies to their latest stable versions: ASP.NET Core / EF Core packages
  (10.0.7 -> 10.0.9), Swashbuckle (10.1.7 -> 10.2.3), `YandexMusic` / `YandexMusic.DependencyInjection`
  (0.1.0 -> 0.2.0), `Flare.Blazor` (0.0.8 -> 0.0.9),
  `System.IdentityModel.Tokens.Jwt` (8.17.0 -> 8.19.1), and the GitHub Actions used in CI.

### Fixed

- `YandexMusic` 0.2.0 fixes a cross-platform bug where requests were sent to `file://` instead of the
  Yandex API host inside Linux containers, which caused every Yandex data call (playlists, tracks,
  search) to fail with a 500 in production while working locally on Windows.
- `Flare.Blazor` 0.0.9 fixes `FlarePasswordField` not propagating its typed value, which made the
  login and registration forms always see an empty password; the app now uses `FlarePasswordField`
  directly again instead of the interim `FlareField Type="password"` workaround.
- The Docker publish workflow (`.github/workflows/docker.yml`) still referenced the pre-restructure
  build context (`src`); corrected to the repository root to match the current Dockerfiles.

### Added

- GitHub Actions CI (build & test), CodeQL analysis, and a Docker image publish workflow for the API
  and PWA.
- Dependabot configuration, issue/PR templates, and community health files (LICENSE, SECURITY,
  CONTRIBUTING, CODE_OF_CONDUCT).

### Removed

- The Gitea remote, the private Gitea NuGet feed, and the credential that was previously committed in
  `nuget.config`. The repository is now prepared for publication on GitHub.

### Security

- Replaced real Yandex OAuth credentials and the database password in `appsettings.json` with
  placeholders; real values must now be supplied via User Secrets or environment variables.
