using Microsoft.AspNetCore.Components;
using PlaylistShare.Pwa.Services;

namespace PlaylistShare.Pwa.Components.Global;

public class ContextualBarContent : ComponentBase, IDisposable
{
    [Inject]
    public ContextualActionBarService ContextualActionBarService { get; set; } = default!;

    [Parameter]
    public ContextualActionBarPosition Position { get; set; } = ContextualActionBarPosition.Default;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnParametersSet()
    {
        ContextualActionBarService.Content = ChildContent;
        ContextualActionBarService.Position = Position;
        ContextualActionBarService.ChangeParameters();
    }

    public void Dispose()
    {
        ContextualActionBarService.Content = null;
        ContextualActionBarService.Position = ContextualActionBarPosition.Default;
        ContextualActionBarService.ChangeParameters();
    }
}