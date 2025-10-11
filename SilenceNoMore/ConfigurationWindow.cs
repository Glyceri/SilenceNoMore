using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Numerics;

namespace SilenceNoMore;

internal class ConfigurationWindow : Window
{
    private readonly Configuration           Configuration;
    private readonly IDalamudPluginInterface DalamudPlugin;

    private readonly Vector2 MinSize     = new Vector2(200, 86);
    private readonly Vector2 MaxSize     = new Vector2(200, 86);
    private readonly Vector2 DefaultSize = new Vector2(200, 86);

    public ConfigurationWindow(IDalamudPluginInterface plugin, IPluginLog log, Configuration configuration) : base("Silence No More", ImGuiWindowFlags.NoResize, true)
    {
        DalamudPlugin = plugin;
        Configuration = configuration;

        SizeCondition = ImGuiCond.FirstUseEver;
        Size = DefaultSize;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
    }

    public override void Draw()
    {
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Coffee.ToIconString()))
        {
            Util.OpenLink("https://ko-fi.com/glyceri");
        }

        ImGui.PopFont();

        ImGui.SameLine();

        ImGui.Text("Ko-Fi for support.");

        if (ImGui.Checkbox("Plugin Enabled", ref Configuration.Enabled))
        {
            Configuration.Save(DalamudPlugin);
        }
    }
}
