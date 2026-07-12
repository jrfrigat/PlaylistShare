using Flare.Abstractions;
using PlaylistShare.Shared;
using PlaylistShare.Shared.Yandex;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PlaylistShare.Pwa.Services;

public class AudioPlayerService : IAudioPlayerService
{
    private readonly TokenStorage _tokenStorage;
    private readonly ISnackbarService _snackbar;
    private readonly HttpClient _http;
    private readonly PlayerStorage _playerStorage;

    private string? _currentTrackId;
    private YandexTrack? _currentTrack;
    private bool _isPlaying;
    private double _currentVolume = 50;
    private double _currentProgress;
    private double _currentTime = 0;
    private double _totalTime = 0;
    private string _currentTimeString = "0:00";
    private string _totalTimeString = "0:00";

    private List<YandexTrack> _queue = new();
    private int _queueIndex = -1;
    private string? _queueShareToken;

    public string? CurrentTrackId => _currentTrackId;
    public YandexTrack? CurrentTrack => _currentTrack;
    public bool IsPlaying => _isPlaying;
    public IReadOnlyList<YandexTrack> CurrentQueue => _queue;
    public bool HasNext => _queueIndex >= 0 && _queueIndex < _queue.Count - 1;
    public bool HasPrevious => _queueIndex > 0;
    public double CurrentVolume
    {
        get => _currentVolume;
        set
        {
            _currentVolume = value;
            OnStateChanged?.Invoke();
        }
    }
    public double CurrentProgress => _currentProgress;
    public double CurrentTime => _currentTime;
    public double TotalTime => _totalTime;
    public string CurrentTimeString => _currentTimeString;
    public string TotalTimeString => _totalTimeString;

    public event Action? OnStateChanged;
    public event Action? OnStartedTrack;
    public event Action? OnEndedTrack;

    public AudioPlayerService(TokenStorage tokenStorage, ISnackbarService snackbar, HttpClient httpClient, PlayerStorage playerStorage)
    {
        _tokenStorage = tokenStorage;
        _snackbar = snackbar;
        _http = httpClient;
        _playerStorage = playerStorage;

        _ = LoadVolume();
    }

    private async Task LoadVolume()
    {
        var savedVolume = await _playerStorage.GetVolumeAsync();
        if (savedVolume != null)
            _currentVolume = savedVolume.Value;
    }

    public async Task LoadAndPlayAsync(string trackId, string? accessToken = null, string? playlistShareToken = null, YandexTrack? track = null)
    {
        if (_currentTrackId == trackId)
        {
            await PlayAsync();
            return;
        }

        _currentTrackId = trackId;

        var idx = _queue.FindIndex(t => t.TrackId == trackId);
        if (idx >= 0) _queueIndex = idx;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            var tokens = await _tokenStorage.GetTokensAsync();
            accessToken = tokens.token;
        }

        string? playToken = null;
        if (!string.IsNullOrWhiteSpace(accessToken))
            playToken = await FetchPlayTokenAsync(accessToken);

        if (string.IsNullOrWhiteSpace(playToken) && string.IsNullOrWhiteSpace(playlistShareToken))
        {
            _snackbar.Show("Не удалось воспроизвести трек: отсутствует токен авторизации или идентификатор расшаренного плейлиста.", SnackbarSeverity.Error);
            return;
        }

        if (track is null)
        {
            try
            {
                track = await GetTrackInfo(trackId, playToken, playlistShareToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch track info: {ex.Message}");
            }
        }

        _currentTrack = track;
        _isPlaying = true;
        OnStateChanged?.Invoke();
        OnLoadAndPlayRequested?.Invoke(trackId, playToken, playlistShareToken);
        OnStartedTrack?.Invoke();
    }

    public async Task PlayAsync()
    {
        _isPlaying = true;
        OnStateChanged?.Invoke();
        OnPlayRequested?.Invoke();
        OnStartedTrack?.Invoke();
    }

    public async Task PauseAsync()
    {
        _isPlaying = false;
        OnStateChanged?.Invoke();
        OnPauseRequested?.Invoke();
    }

    public async Task SeekToAsync(double second)
    {
        // Оптимистично двигаем прогресс сразу - не дожидаясь timeupdate от аудио, иначе полоса
        // "догоняет" новую позицию с заметной задержкой после перемотки.
        _currentTime = second;
        _currentTimeString = FormatDuration(second);
        if (_totalTime > 0)
            _currentProgress = (second / _totalTime) * 100;
        OnStateChanged?.Invoke();

        OnSeekRequested?.Invoke(second);
    }

    public async Task SetVolumeAsync(double volume)
    {
        _currentVolume = volume;
        OnStateChanged?.Invoke();
        OnVolumeChangeRequested?.Invoke(volume);
        await _playerStorage.SetVolumeAsync(volume);
    }

    public void SetQueue(IEnumerable<YandexTrack> tracks, int startIndex = 0, string? shareToken = null)
    {
        _queue = tracks.ToList();
        _queueIndex = _queue.Count > 0 ? Math.Clamp(startIndex, 0, _queue.Count - 1) : -1;
        _queueShareToken = shareToken;
        OnStateChanged?.Invoke();
    }

    public async Task PlayNextAsync()
    {
        if (!HasNext) return;
        _queueIndex++;
        var track = _queue[_queueIndex];
        await LoadAndPlayAsync(track.TrackId, playlistShareToken: _queueShareToken, track: track);
    }

    public async Task PlayPreviousAsync()
    {
        if (!HasPrevious) return;
        _queueIndex--;
        var track = _queue[_queueIndex];
        await LoadAndPlayAsync(track.TrackId, playlistShareToken: _queueShareToken, track: track);
    }

    public event Func<string, string?, string?, Task>? OnLoadAndPlayRequested;
    public event Func<Task>? OnPlayRequested;
    public event Func<Task>? OnPauseRequested;
    public event Func<double, Task>? OnSeekRequested;
    public event Func<double, Task>? OnVolumeChangeRequested;

    public void SetPlayingState(bool isPlaying)
    {
        _isPlaying = isPlaying;
        OnStateChanged?.Invoke();
    }

    public void SetCurrentTrack(string? trackId)
    {
        _currentTrackId = trackId;
        OnStateChanged?.Invoke();
    }

    public void UpdateProgress(double currentTime, double totalTime)
    {
        var progress = (currentTime / totalTime) * 100;
        _currentProgress = progress;
        _currentTime = currentTime;
        _currentTimeString = FormatDuration(currentTime);
        _totalTime = totalTime;
        _totalTimeString = FormatDuration(totalTime);
        OnStateChanged?.Invoke();
    }

    public void NotifyTrackEnded()
    {
        _isPlaying = false;
        _currentProgress = 0;
        _currentTime = 0;
        _currentTimeString = "0:00";
        _totalTime = 0;
        _totalTimeString = "0:00";
        OnStateChanged?.Invoke();
        OnEndedTrack?.Invoke();

        if (HasNext)
            _ = PlayNextAsync();
        else
            _currentTrackId = null;
    }

    private async Task<string?> FetchPlayTokenAsync(string jwt)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/audio/play-token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return result?.Data;
    }

    private async Task<YandexTrack?> GetTrackInfo(string trackId, string? playToken, string? sharedPlaylistId)
    {
        var url = $"/api/audio/track-info/{trackId}";
        if (!string.IsNullOrEmpty(playToken))
            url += $"?play_token={playToken}";
        else if (!string.IsNullOrEmpty(sharedPlaylistId))
            url += $"?shared_id={sharedPlaylistId}";

        var response = await _http.GetFromJsonAsync<ApiResponse<YandexTrack>>(url);
        return response?.Success == true ? response.Data : null;
    }

    private string FormatDuration(double seconds)
    {
        var mins = (int)(seconds / 60);
        var secs = (int)(seconds % 60);
        return $"{mins}:{secs:D2}";
    }
}
