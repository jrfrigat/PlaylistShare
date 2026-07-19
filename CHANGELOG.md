# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2026-07-19

### Changed

- **Updated Flare to 0.10.0 and moved icons to inline SVG.** Icons no longer come from the Material
  Symbols webfont, so its download is gone - there is no longer a flash of missing icons on the first
  load, and one fewer network request at startup. Play/pause and the favorite star draw the filled
  icon variant directly.

## [1.2.0] - 2026-07-17

A backend release. Most of it is invisible from the outside - the data layer moved into its own
projects, the model is configured with attributes, requests are validated, and errors are logged
instead of vanishing. The visible part is access control: editing a shared playlist now needs the
right to view it, and a shared computer no longer lets one person touch another's tracks.

### Added

- **Requests are validated at the boundary** - a value that would not fit the database is rejected
  with a 400 instead of failing deep inside a save.
- **Unhandled errors now carry a trace id** and are written to the log. Before, an unexpected error
  reached the client as an empty 500 and left nothing behind to trace it by.
- **The database connection can retry** on a transient failure (`Database:MaxRetryCount` /
  `Database:MaxRetryDelay`, both off by default, so nothing changes unless you set them).

### Changed

- **The database model and migrations moved into their own projects** (`PlaylistShare.Database` plus a
  migration project per provider). There is now one `DbContext` instead of a subclass per provider,
  and the provider is chosen by configuration. An unknown provider name now fails loudly at startup
  instead of quietly falling back to SQL Server.
- **The EF model is configured with attributes on the entities** rather than in `OnModelCreating`.
- **Removed the `UserSessions` table.** Nothing read it, and it was gaining a row on every request
  because the session id was never stable. The track logs keep the session id as a plain value.
- **Requests are cancelled properly** - if the caller goes away, the work against the database and
  Yandex stops instead of running to completion.
- Updated Flare from 0.6.0 to **0.8.0**.

### Fixed

- **A 500 right after signing in with QR.** The sign-in row was marked confirmed and then deleted in
  the same step, and the next save tried to update a row that was already gone.
- **A playlist title longer than 255 characters crashed creation.** Titles come from Yandex, not from
  the client, so they are now truncated to the column length instead of failing the save.
- **"Only who added it can remove it" now works for anonymous listeners.** The session id used to
  change on every request, so the match never held; it is stable now.
- Removed dead code (an unused request type that was never wired to anything).

### Security

- **Editing a shared playlist now requires the right to view it.** With view limited to signed-in
  users but adding open to everyone, a stranger with the link could change a playlist they were not
  even allowed to open.
- **A shared browser session no longer unlocks someone else's track.** On a shared computer the next
  person to sign in could remove the previous one's track, because a session match beat a different
  account. A session now only counts for tracks that were added anonymously.

## [1.1.0] - 2026-07-17

A player release: the mini player is half the height it was, the progress bar is a thin line at its
edge rather than a row of its own, and the whole bar background is now the scrub area on desktop.
Underneath, several things that had never actually worked in production are fixed - PWA updates,
static-file caching, and the volume slider.

### Added

- **Seek from the mini player background (desktop).** Clicking anywhere on the bar that is not a
  control jumps to that point in the track; the title still opens the full-screen player. On phones
  the bar behaves as before - any tap opens the full player, where the progress bar is easy to hit.

### Changed

- **The mini player is now 60px tall** (was 115px). The progress bar sits flush against the bottom
  edge and no longer takes up a row of its own.
- **Player sliders look like a music player**: a thin track with no handle in the mini player, where
  the background fill already shows the position. They previously used the Material Design 3
  Expressive shape - a 16px track with a 44px handle - which filled most of the bar.
- **The playlist buttons moved next to the cover**, under the description, on desktop.
- Updated Flare from 0.2.1 to **0.6.0**.

### Fixed

- **PWA updates never reached anyone.** The published build shipped Blazor's *development* service
  worker - a stub that does nothing - because the paths in the project file had doubled backslashes,
  so the swap to the real one silently never happened. The app had no caching, no offline support and
  no update prompt. New versions now announce themselves and apply on click.
- **Static files were served as `immutable` for a year**, so style changes never reached anyone who
  had opened the site before. Only files with a hash in their name are cached that way now.
- **An "unhandled error" banner covered the player and swallowed every click.** Nothing was crashing:
  Blazor WebAssembly treats anything written to stderr as a crash signal, and a routine 401 for an
  anonymous visitor was enough to trigger it.
- **The volume could not be adjusted.** The panel opens above the bar, and the bar clipped it away -
  it was there the whole time, just unreachable. It also no longer closes while the cursor is on the
  slider.
- **Seeking landed on the wrong second.** The track length reached the DOM with a decimal comma, so
  the browser rejected it and scaled the whole bar to 100 - on a 146-second track, clicking the
  middle jumped to 0:50 instead of 1:13.
- **The slider track was invisible** in both players: only the handle drew. A gap value was written
  without its unit, which invalidates the calculation that positions the track.
- **The full-screen player was static**: the progress bar and the elapsed time never moved, the fill
  was invisible against the cover, and the colours kept the previous track's palette after skipping.
- **The saved volume was ignored on startup** - playback always began at full volume while the slider
  showed the stored value.
- The progress bar and the background fill disagreed about the position everywhere except 0% and 100%.

## [1.0.1] - 2026-07-14

Points at the same commit as 1.0.0 - no source changes.

## [1.0.0] - 2026-07-14

First public release: PlaylistShare on GitHub, with both Docker images published to GHCR and Docker
Hub.

### Added

- Selectable database provider: **SQL Server (default) or PostgreSQL**, chosen via `Database:Provider`
  (env `Database__Provider`; `SqlServer` | `Postgres`). Each provider carries its own EF Core
  migration set, and the session cache lives in the chosen database. Two ways to run Postgres in
  Docker: an external instance (`DB_PROVIDER=Postgres` + a Postgres `CONNECTION_STRING` in `.env`) or a
  containerized one via `docker-compose.postgres.yml` (`run-docker-compose-postgres.bat`).
- GitHub Actions CI (build & test), CodeQL analysis, and a Docker image publish workflow for the API
  and PWA.
- Dependabot configuration, issue/PR templates, and community health files (LICENSE, SECURITY,
  CONTRIBUTING, CODE_OF_CONDUCT).

### Changed

- Rebranded the product to **PlaylistShare** and rewrote the README (EN + RU) and changelog to match
  the current architecture, database options, and Docker workflow.
- Restructured the repository to a single-root layout (solution and tooling at the root, projects
  under `src/`), mirroring the sibling Flare repository.
- Migrated the PWA UI from MudBlazor to [Flare](https://github.com/jrfrigat/Flare) (`Flare.Blazor`).
- Replaced the generic Material Design 3 theme with a bespoke "Deka" Flare theme (own OKLCH palette,
  Onest typography, rounder shape scale) ported from the Deka Playlist Share design, and restyled every
  PWA page/component to match it; the PWA now references `Flare.Theme.MaterialDesign3Expressive` (for its
  design-token baseline) instead of `Flare.Theme.MaterialDesign3`.
- Migrated the Yandex Music integration from the legacy private `YandexMusic.API` package to the new
  [YandexMusic](https://github.com/jrfrigat/YandexMusic) client.
- Backend optimization pass across `PlaylistShare.Api` (startup, DI, and data-access cleanup).
- Moved the Data Protection key ring out of the database onto a **mounted volume** (`DataProtection:KeysPath`,
  `/keys` in Docker), so keys survive redeploys independently of the database.
- Externalized production secrets to a git-ignored `.env` file, applied through the
  `docker-compose.prod.yml` overlay (see `.env.example`).
- Updated all dependencies to their latest stable versions: ASP.NET Core / EF Core packages
  (10.0.7 -> 10.0.9), Swashbuckle (10.1.7 -> 10.2.3), `YandexMusic` / `YandexMusic.DependencyInjection`
  (0.1.0 -> 0.2.0), `Flare.Blazor` (0.0.8 -> 0.0.9),
  `System.IdentityModel.Tokens.Jwt` (8.17.0 -> 8.19.1), and the GitHub Actions used in CI.

### Removed

- Unused **Keycloak / OpenID** integration and the **Yandex OAuth** client credentials
  (`Yandex:ClientId` / `Yandex:ClientSecret`). Yandex Music accounts are linked via QR sign-in or by
  pasting an access token; there is no "Sign in with Yandex" OAuth flow.
- The Gitea remote, the private Gitea NuGet feed, and the credential that was previously committed in
  `nuget.config`. The repository is now prepared for publication on GitHub.

### Fixed

- `YandexMusic` 0.2.0 fixes a cross-platform bug where requests were sent to `file://` instead of the
  Yandex API host inside Linux containers, which caused every Yandex data call (playlists, tracks,
  search) to fail with a 500 in production while working locally on Windows.
- `Flare.Blazor` 0.0.9 fixes `FlarePasswordField` not propagating its typed value, which made the
  login and registration forms always see an empty password; the app now uses `FlarePasswordField`
  directly again instead of the interim `FlareField Type="password"` workaround.
- The Docker publish workflow (`.github/workflows/docker.yml`) still referenced the pre-restructure
  build context (`src`); corrected to the repository root to match the current Dockerfiles.

### Security

- Replaced real Yandex OAuth credentials and the database password in `appsettings.json` with
  placeholders; real values must now be supplied via User Secrets or environment variables.
