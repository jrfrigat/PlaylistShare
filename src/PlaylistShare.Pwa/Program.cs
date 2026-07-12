using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Flare.Extensions;
using PlaylistShare.Pwa;
using PlaylistShare.Pwa.Services;
using PlaylistShare.Pwa.Theming;

internal class Program
{
    private static async global::System.Threading.Tasks.Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddFlare(opts => opts.DefaultTheme = new DekaTheme());
        builder.Services.AddFlareTheme(new DekaTheme());
        builder.Services.AddScoped(sp =>
        {
            var apiUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
            return new HttpClient { BaseAddress = new Uri(apiUrl) };
        });

        // Обновление PWA - встроенный механизм Flare: опрашивает развёрнутую версию (читает
        // service-worker-assets.js мимо кешей), сам регистрирует SW и умеет активировать ждущий
        // воркер + перезагрузиться. MainLayout подписывается на NewVersionAvailable и показывает плашку.
        builder.Services.AddFlareVersionCheck(opts =>
        {
            opts.UseServiceWorker = true;
            opts.CheckImmediately = true;             // проверить новую версию сразу при загрузке
            opts.Interval = TimeSpan.FromMinutes(1);  // и далее раз в минуту для долго открытых вкладок
        });
        builder.Services.AddScoped<TokenStorage>();
        builder.Services.AddScoped<PlayerStorage>();
        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
        builder.Services.AddScoped<ContextualActionBarService>();
        builder.Services.AddScoped<AppBarState>();
        builder.Services.AddScoped<ApiClient>();
        builder.Services.AddScoped<IAudioPlayerService, AudioPlayerService>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();

        await builder.Build().RunAsync();
    }
}