using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System.Numerics;

namespace SilenceNoMore.Windowing;

internal abstract class SilenceNoMoreWindow : Window
{
    private static readonly Vector2 windowPadding    = new Vector2(8, 8);
    private static readonly Vector2 framePadding     = new Vector2(4, 3);
    private static readonly Vector2 itemInnerSpacing = new Vector2(4, 4);
    private static readonly Vector2 itemSpacing      = new Vector2(4, 4);

    protected abstract Vector2 MinSize     { get; }
    protected abstract Vector2 MaxSize     { get; }
    protected abstract Vector2 DefaultSize { get; }

    private float lastGlobalScale = 0;

    protected SilenceNoMoreWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) 
        : base(name, flags, forceMainWindow) 
    {
        SetSizeConstraints();
    }

    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    windowPadding    * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     framePadding     * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,      itemSpacing      * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, itemInnerSpacing * WindowHandler.GlobalScale);

        float currentGlobalScale = WindowHandler.FontScale;

        if (lastGlobalScale == currentGlobalScale)
        {
            return;
        }

        lastGlobalScale = currentGlobalScale;

        SetSizeConstraints();
    }

    public sealed override void PostDraw()
    {
        ImGui.PopStyleVar(4);
    }

    private void SetSizeConstraints()
    {
        float currentGlobalScale = WindowHandler.FontScale;

        SizeCondition   = ImGuiCond.FirstUseEver;
        Size            = DefaultSize * currentGlobalScale;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize * currentGlobalScale,
            MaximumSize = MaxSize * currentGlobalScale,
        };
    }
}
