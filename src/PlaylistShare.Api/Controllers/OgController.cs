using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Services;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("shared")]
public class OgController : ControllerBase
{
    private readonly SharedPlaylistService _sharedService;
    private readonly string _clientBaseUrl;

    public OgController(SharedPlaylistService sharedService, IConfiguration configuration)
    {
        _sharedService = sharedService;
        _clientBaseUrl = configuration["Client:BaseUrl"]?.TrimEnd('/') ?? "";
    }

    [HttpGet("{token}")]
    [Produces("text/html")]
    public async Task<ContentResult> GetOgPage(string token, CancellationToken cancellationToken)
    {
        var entity = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        var pwaUrl = $"{_clientBaseUrl}/shared/{token}";

        string title = entity?.Title ?? "Playlist Share";
        string description = entity != null
            ? $"Слушайте плейлист '{entity.Title}' на Playlist Share"
            : "Совместные плейлисты Яндекс.Музыки";
        string imageUrl = !string.IsNullOrEmpty(entity?.CoverUrl) ? entity.CoverUrl : "";

        var html = $"""
            <!DOCTYPE html>
            <html lang="ru">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>{HtmlEncode(title)}</title>

                <meta property="og:type"        content="music.playlist" />
                <meta property="og:title"       content="{HtmlEncode(title)}" />
                <meta property="og:description" content="{HtmlEncode(description)}" />
                <meta property="og:url"         content="{HtmlEncode(pwaUrl)}" />
                {(string.IsNullOrEmpty(imageUrl) ? "" : $"""<meta property="og:image" content="{HtmlEncode(imageUrl)}" />""")}
                <meta name="twitter:card"        content="summary_large_image" />
                <meta name="twitter:title"       content="{HtmlEncode(title)}" />
                <meta name="twitter:description" content="{HtmlEncode(description)}" />
                {(string.IsNullOrEmpty(imageUrl) ? "" : $"""<meta name="twitter:image" content="{HtmlEncode(imageUrl)}" />""")}

                <meta http-equiv="refresh" content="0;url={HtmlEncode(pwaUrl)}" />
                <script>window.location.replace("{JsEncode(pwaUrl)}");</script>
            </head>
            <body>
                <p>Перенаправление на <a href="{HtmlEncode(pwaUrl)}">{HtmlEncode(title)}</a>...</p>
            </body>
            </html>
            """;

        return Content(html, "text/html; charset=utf-8");
    }

    private static string HtmlEncode(string s) =>
        System.Web.HttpUtility.HtmlAttributeEncode(s);

    private static string JsEncode(string s) =>
        s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
}
