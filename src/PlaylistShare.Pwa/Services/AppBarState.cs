namespace PlaylistShare.Pwa.Services;

/// <summary>
/// Состояние мобильного верхнего app bar (в духе MD3): страница задаёт заголовок и, при необходимости,
/// кнопку "назад". MainLayout рисует его в мобильном хедере. На десктопе хедер скрыт - состояние
/// игнорируется. Страница должна вызвать <see cref="Reset"/> при уходе.
/// </summary>
public class AppBarState
{
    /// <summary>Заголовок в app bar. Если null - показываем брендовый логотип "Дека".</summary>
    public string? Title { get; private set; }

    /// <summary>Показывать ли кнопку "назад" слева.</summary>
    public bool ShowBack { get; private set; }

    /// <summary>Действие по кнопке "назад".</summary>
    public Func<Task>? BackAction { get; private set; }

    /// <summary>Вызывается при изменении состояния - MainLayout перерисовывает хедер.</summary>
    public event Action? OnChanged;

    /// <summary>Задать контекст app bar (заголовок + опциональная кнопка "назад").</summary>
    public void Set(string? title, bool showBack = false, Func<Task>? back = null)
    {
        Title = title;
        ShowBack = showBack;
        BackAction = back;
        OnChanged?.Invoke();
    }

    /// <summary>Сбросить к брендовому виду (без заголовка и "назад").</summary>
    public void Reset()
    {
        Title = null;
        ShowBack = false;
        BackAction = null;
        OnChanged?.Invoke();
    }
}
