# BackendMusic

ASP.NET Core 5 Web API backend for a personal music-player app — pairs with the
[`Music`](https://github.com/Pepeeeeeeeeee/Music) Ionic/Vue frontend. Built as a secondary-school
graduation project (SPŠE Bratislava).

## What it does

- **Auth** (`UserController`) — register/login against a MySQL `Users` table.
- **Player** (`PlayerController`) — playlists, favorites, and last-played history, all backed by
  MySQL; plus pulling in new tracks from YouTube:
  - `DownloadAndConvert` — downloads a video (`VideoLibrary`) and converts it to `.mp3` with
    `MediaToolkit`/ffmpeg.
  - `DownloadAndConvertV2` — a leaner path using `YoutubeExplode` to grab the best audio-only
    stream directly, no re-encode.
- Search (on the frontend side) hits the YouTube Data API v3 directly.

## Stack

ASP.NET Core 5, MySql.Data, YoutubeExplode, VideoLibrary + MediaToolkit (ffmpeg wrapper),
Swagger/Swashbuckle.

## Running it

Needs a MySQL server reachable at whatever host you set in the (redacted) connection strings in
`Controllers/*.cs`, with `Users`, `PlayLists`, `PlayList`, `LastPlayed`, and `Favorites` tables.
Database name, user, and password were never committed — fill those in locally before running.

```bash
dotnet run
```

Swagger UI comes up at `/swagger` in development.

## Known limitations

This was student coursework, not production code, and reads like it:

- Passwords are stored and compared in plain text (`Register`/`Login` in `UserController`) — no
  hashing.
- `DownloadAndConvert`/`DownloadAndConvertV2` write to hardcoded absolute Windows paths
  (`E:/Music/mp3`, `e:/MusicSrc/YTVid/`) rather than a configurable location.
- SQL is built with ad-hoc string interpolation in a couple of places (e.g. the `idLP` lookup in
  `LastPlayed`) instead of parameters throughout.
- Comments are a mix of English and Slovak, left as originally written.
