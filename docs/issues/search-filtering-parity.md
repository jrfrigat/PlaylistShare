# Search filtering parity - restore rich search + playlist-track filter dropped in the Deka rework

> **Status: Implemented.** Rebuilt `Search.razor` (type chips, combined "Топ" with tracks first,
> client-side track filter, entity drill-down, kept the `?AddTo=` direct add) and added a
> "Поиск трека" filter to `SharedPlaylistView`. No Flare/YandexMusic change was needed. Kept for
> reference / rationale.


**Type:** app feature gap (PlaylistShare.Pwa UI) - **not** a Flare or YandexMusic library gap.
**Area:** `Pages/Search.razor`, `Pages/SharedPlaylistView.razor`
**Reference UX:** Yandex Music `music.yandex.ru/search?text=...` and a playlist page `music.yandex.ru/playlists/{uuid}`.
**Prior art:** the pre-Flare MudBlazor search lived in `SharedPlaylistView.razor` at commit `7dde55d^`
(before "migrate the Blazor client from MudBlazor to Flare"). It had all of this; the Deka rework
(`02b28e7`) reimplemented search as a 3-tab page and dropped the filtering.

---

## TL;DR

The current `Search.razor` exposes only three tabs - **Треки / Альбомы / Исполнители** - and no
in-list filtering. Yandex Music (and our own old search) offer a combined "Топ" view, more type
filters (incl. Playlists / My playlists), a client-side text filter over the results, entity
drill-down, and - on the playlist page - a **"Поиск трека"** box that filters the playlist's own
tracks. **Everything needed to rebuild this already exists** in the backend, the YandexMusic client,
and Flare - so no library issue is warranted; this is a UI re-implementation.

---

## What's missing (vs. Yandex Music + the old MudBlazor search)

1. **Combined "Все / Топ" view.** Yandex's default "Топ" chip shows a best-match hero plus sections
   (Исполнители -> Альбомы -> Плейлисты -> Треки) in one scroll. Current search has no combined mode -
   only per-type tabs. The old search had `TrackSearchType.All` rendering exactly these sections.

2. **Missing type filters: `Плейлисты` and `Мои плейлисты`.** Yandex filters: Топ / Треки / Плейлисты
   / Исполнители / Альбомы (+ Моя волна / Подкасты / Концерты / Аудиокниги / Клипы - out of scope for a
   music-playlist app). Ours has only Треки / Альбомы / Исполнители. The old search additionally had
   `Плейлисты` and `Мои плейлисты`.

3. **Client-side filter over the track results.** The old search rendered a `Фильтр треков...` text field
   above the returned tracks (`_searchFilterText` -> `FilteredSearchTracks`, matching title/artist,
   case-insensitive). Current search has none.

4. **Entity drill-down.** In the old search, clicking an artist / album / playlist card searched that
   entity's tracks (`SearchTracksByEntity(id, name, type)` -> `byId=true`). Current `Search.razor`
   renders `AlbumCard`/`ArtistCard` without a drill-down action.

5. **Playlist-page track filter.** Yandex's playlist page has a **"Поиск трека"** box directly under
   the header that filters the playlist's own tracks. `SharedPlaylistView` has no such box. The old
   view had it (`_playlistFilterText` -> `FilteredPlaylistTracks`).

---

## Why this is *not* a Flare / YandexMusic gap (already supported)

**Backend** (`Api/Controllers/YandexSearchController.cs`, `Services/Yandex/YandexMusicService.cs`):
- `GET /api/yandexsearch/search` already accepts `searchType` = `All | Artist | Album | Playlist |
  Track | MyPlaylists`, plus `byId=true` (drill-down) and `shared_id`.
- `SearchAsync` maps to `Client.Search.SearchAsync(query, SearchType.*)`; `SearchMyPlaylists` and
  `SearchTracksByIdAsync` exist.
- `Shared/Yandex/YandexSearchResult.cs` already carries `Tracks / Playlists / PersonalPlaylists /
  LikedPlaylists / Artists / Albums`.

**YandexMusic client:** `Client.Search.SearchAsync(query, SearchType.All/Artist/Album/Playlist/Track)`
and `Client.Playlists.GetByUuidAsync / GetByUserAsync` cover every case above.

**Flare (0.1.2):**
- `FlareChipGroup` (single-select via `MultiSelect=false`, `SelectedValues`/`SelectedValuesChanged`)
  -> the Yandex-style scrollable filter-chip bar. `chipgroup.css` ships in 0.1.2.
- `FlareField` -> the `Фильтр треков...` / `Поиск трека` boxes.
- `FlareTabs`, `FlareStack`, existing `AlbumCard` / `ArtistCard` / `PlaylistCard` -> sections & drill-down.

No component is missing; horizontal-scroll of the chip bar is a one-line `overflow-x:auto` wrapper.

---

## Implementation sketch

`Search.razor`
- Replace the 3-tab `FlareTabs` with a scrollable `FlareChipGroup` (single-select):
  `Топ(All) / Треки / Плейлисты / Исполнители / Альбомы / Мои плейлисты`.
- For `All`, render the combined sections (best match + Исполнители/Альбомы/Плейлисты/Треки) - reuse the
  old `SharedPlaylistView@7dde55d^` layout as the blueprint.
- Add a `FlareField` "Фильтр треков..." above the track list; filter client-side by title/artist.
- Wire `AlbumCard`/`ArtistCard`/`PlaylistCard` `OnClick` -> re-search with `byId=true` for that entity.
- Keep the existing `?AddTo={token}` behaviour (per-track add/remove to the target playlist).

`SharedPlaylistView.razor`
- Add a `FlareField` "Поиск трека" under the hero; filter `_tracks` client-side by title/artist
  (case-insensitive), same as the old `FilteredPlaylistTracks`.

## Out of scope
Podcasts / audiobooks / clips / concerts search types (Yandex offers them; irrelevant to a
music-playlist sharing app).
