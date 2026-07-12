export function init(audioId, dotNetHelper) {
    const audio = document.getElementById(audioId);
    if (!audio) throw new Error(`Audio element with id ${audioId} not found`);

    let durationReady = false;
    let durationValue = 0;

    const toNumber = (val) => {
        const num = Number(val);
        return isNaN(num) ? 0 : num;
    };

    const loadAndPlay = (src, token, sharedPlaylistId) => {
        const url = new URL(src, window.location.href);
        if (token) url.searchParams.set('play_token', token);
        if (sharedPlaylistId) url.searchParams.set('shared_id', sharedPlaylistId);
        audio.src = url.toString();
        audio.load();
        durationReady = false;
        durationValue = 0;
        audio.play().catch(e => console.error('Play failed:', e));
    };

    const play = () => audio.play();
    const pause = () => audio.pause();
    const stop = () => { audio.pause(); audio.currentTime = 0; };
    const setVolume = (volume) => { audio.volume = toNumber(volume); };
    const setCurrentTime = (time) => { audio.currentTime = toNumber(time); };

    audio.addEventListener('loadedmetadata', () => {
        const current = toNumber(audio.currentTime);
        durationValue = toNumber(audio.duration);
        durationReady = durationValue > 0;
        if (dotNetHelper && durationReady) {
            dotNetHelper.invokeMethodAsync('OnTimeUpdate', current, durationValue);
        }
    });

    audio.addEventListener('timeupdate', () => {
        if (dotNetHelper && durationReady) {
            const current = toNumber(audio.currentTime);
            dotNetHelper.invokeMethodAsync('OnTimeUpdate', current, durationValue);
        }
    });

    audio.addEventListener('ended', () => {
        if (dotNetHelper) {
            dotNetHelper.invokeMethodAsync('OnAudioEnded');
        }
    });

    audio.addEventListener('progress', () => {
        if (dotNetHelper) {
            if (audio.buffered.length > 0 && audio.duration) {
                const bufferedEnd = toNumber(audio.buffered.end(audio.buffered.length - 1));
                dotNetHelper.invokeMethodAsync('OnDownloadProgress', bufferedEnd);
            }
        }
    });

    const handleKeyDown = (e) => {
        const tag = e.target.tagName.toLowerCase();
        if (tag === 'input' || tag === 'textarea' || e.target.isContentEditable) return;

        switch (e.key) {
            case ' ':
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnKeyboardTogglePlay');
                break;
            case 'ArrowLeft':
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnKeyboardSeek', -10);
                break;
            case 'ArrowRight':
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnKeyboardSeek', 10);
                break;
            case 'ArrowUp':
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnKeyboardVolumeChange', 5);
                break;
            case 'ArrowDown':
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnKeyboardVolumeChange', -5);
                break;
        }
    };
    window.addEventListener('keydown', handleKeyDown);

    const destroy = () => window.removeEventListener('keydown', handleKeyDown);

    return { loadAndPlay, play, pause, stop, setVolume, setCurrentTime, destroy };
}