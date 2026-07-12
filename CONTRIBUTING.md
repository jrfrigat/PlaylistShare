# Contributing to PlaylistShare

Thanks for your interest in improving PlaylistShare! This guide covers everything you need to get a
change merged.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- The Blazor WebAssembly tools: `dotnet workload install wasm-tools`
- A recent IDE (Visual Studio 2022/2026, Rider, or VS Code with the C#/Razor extensions)
- For the API: SQL Server or PostgreSQL - selectable via `Database:Provider` (or use the bundled
  `docker-compose` for a local instance)

## Build & test

The repository builds from a single solution:

```sh
dotnet restore PlaylistShare.slnx
dotnet build   PlaylistShare.slnx -c Release
dotnet test    PlaylistShare.slnx -c Release
```

Run the app locally:

```sh
# API (https://localhost:8081 by default)
dotnet run --project src/PlaylistShare.Api

# PWA client
dotnet run --project src/PlaylistShare.Pwa
```

Or bring the whole stack up with Docker:

```sh
docker compose up -d --build
```

> **Note on the YandexMusic client.** `PlaylistShare.Api` depends on the
> [YandexMusic](https://github.com/jrfrigat/YandexMusic) client, referenced as the published
> [`YandexMusic`](https://www.nuget.org/packages/YandexMusic) NuGet package. Improvements to that
> client ship as new package versions. See `src/PlaylistShare.Api/PlaylistShare.Api.csproj`.

## Configuration & secrets

**Never commit real secrets.** `appsettings.json` ships with placeholders only. Provide real values
through [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for local
development or environment variables in production:

```sh
cd src/PlaylistShare.Api
dotnet user-secrets set "Jwt:Key" "..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

## Workflow

1. Fork the repo and create a short-lived branch off `main` (e.g. `feat/share-link`,
   `fix/track-toggle`).
2. Make your change. Add or update an EF Core migration if the data model changes.
3. Make sure `dotnet build` and `dotnet test` pass, and that CI is green on your PR.
4. Open a pull request against `main`. PRs are squash-merged, so please use
   [Conventional Commits](https://www.conventionalcommits.org/) style for the title
   (`feat(api): ...`, `fix(pwa): ...`, `docs: ...`).

`main` is always deployable; releases are cut by tagging `vX.Y.Z`, which builds and publishes the
API and PWA Docker images to the GitHub Container Registry.

## Reporting bugs & requesting features

Use the [issue templates](https://github.com/jrfrigat/PlaylistShare/issues/new/choose). For security
issues, see [SECURITY.md](SECURITY.md) - please do not open a public issue.

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
