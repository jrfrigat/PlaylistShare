using Microsoft.JSInterop;

namespace PlaylistShare.Pwa.Services;

public class PlayerStorage
{
    private readonly IJSRuntime _js;
    private const string VolumeKey = "audio_player_volume";

    public PlayerStorage(IJSRuntime js) => _js = js;

    public async Task SetVolumeAsync(double volume)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", VolumeKey, volume);
    }

    public async Task<double?> GetVolumeAsync()
    {
        var volume = await _js.InvokeAsync<string>("localStorage.getItem", VolumeKey);

        if (double.TryParse(volume, out var result))
        {
            result = Math.Clamp(result, 0, 100);

            return result;
        }

        return null;
    }
}