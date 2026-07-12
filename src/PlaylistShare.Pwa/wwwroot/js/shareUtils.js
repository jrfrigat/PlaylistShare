export function isSupported() {
    return !!navigator.share;
}

export async function shareLink(title, text, url) {
    if (!navigator.share) {
        return { success: false, error: 'Web Share API не поддерживается' };
    }
    try {
        await navigator.share({ title, text, url });
        return { success: true };
    } catch (error) {
        if (error.name === 'AbortError') {
            return { success: false, cancelled: true };
        }
        console.error('Ошибка при шеринге:', error);
        return { success: false, error: error.message };
    }
}