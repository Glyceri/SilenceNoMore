using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Numerics;

namespace SilenceNoMore.Windowing;

internal class ConfigurationWindow : Window
{
    private readonly Configuration              Configuration;
    private readonly IPluginLog                 Log;
    private readonly IDalamudPluginInterface    DalamudPlugin;
    private readonly WindowHandler              WindowHandler;

    private readonly Vector2 MinSize     = new Vector2(200, 132);
    private readonly Vector2 MaxSize     = new Vector2(400, 300);
    private readonly Vector2 DefaultSize = new Vector2(200, 132);

    public ConfigurationWindow(IDalamudPluginInterface plugin, IPluginLog log, Configuration configuration, WindowHandler windowHandler) 
        : base("Silence No More", ImGuiWindowFlags.None, true)
    {
        DalamudPlugin       = plugin;
        Log                 = log;
        Configuration       = configuration;
        WindowHandler       = windowHandler;

        SizeCondition       = ImGuiCond.FirstUseEver;
        Size                = DefaultSize;

        SizeConstraints     = new WindowSizeConstraints()
        {
            MinimumSize     = MinSize,
            MaximumSize     = MaxSize,
        };
    }

    public override void OnClose()
        => WindowHandler.AdvancedConfigurationWindow.IsOpen = false;

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
            Configuration.Save(DalamudPlugin, Log);
        }

        ImGui.NewLine();

        if (ImGui.Button("Open Advanced Settings"))
        {
            WindowHandler.AdvancedConfigurationWindow.IsOpen = true;
        }
    }
}
