using Microsoft.AspNetCore.Components;

namespace PlaylistShare.Pwa.Services;

public class ContextualActionBarService
{
    // Событие, которое будет вызываться при изменении содержимого панели
    public event Action? OnChanged;

    public RenderFragment? Content { get; set; } = null;

    public ContextualActionBarPosition Position { get; set; } = ContextualActionBarPosition.Default;

    public void ChangeParameters()
    {
        OnChanged?.Invoke();
    }
}

public enum ContextualActionBarPosition
{
    Default,
    Top,
    Bottom,
}