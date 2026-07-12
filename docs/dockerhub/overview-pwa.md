# PlaylistShare PWA

Front end for **PlaylistShare** - share and collaborate on Yandex Music playlists. This image is the
compiled **Blazor WebAssembly** app, published to static files and served by **nginx**. It runs entirely
in the browser and talks to the `playlistshare-api` back end over HTTP.

## What's inside

- Blazor WebAssembly output (`wwwroot`) served by `nginx:alpine` with the app's own `nginx.conf`
  (SPA fallback routing).
- Listens on container port **80**. No runtime .NET on the server - it's static assets only.
- Progressive Web App: installable, with an app manifest and service worker.

## Run

```sh
docker run -p 8080:80 frigat/playlistshare-pwa:latest
# Open http://localhost:8080
```

The PWA needs a reachable `playlistshare-api` instance. For a full stack (PWA + API + optional
containerized Postgres), use the Compose files in the repository, which wire the two together on a shared
network.

## Tags

- `latest` - the most recent release.
- `<version>` (e.g. `1.0.0`) - a pinned version (recommended for production).

## Links

- Source, full documentation and issues: https://github.com/jrfrigat/PlaylistShare
- Back end: `playlistshare-api`
- Also published to GHCR: `ghcr.io/jrfrigat/playlistshare-pwa`
- License: MIT
