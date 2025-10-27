using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using SilenceNoMore.Windowing.Components;
using System.Numerics;

namespace SilenceNoMore.Windowing;

internal class ConfigurationWindow : SilenceNoMoreWindow
{
    private readonly Configuration              Configuration;
    private readonly IPluginLog                 Log;
    private readonly IDalamudPluginInterface    DalamudPlugin;
    private readonly WindowHandler              WindowHandler;

    private static readonly Vector2 WindowSize = new Vector2(230, 206);

    public ConfigurationWindow(IDalamudPluginInterface plugin, IPluginLog log, Configuration configuration, WindowHandler windowHandler) 
        : base("Silence No More", ImGuiWindowFlags.NoResize, true)
    {
        DalamudPlugin       = plugin;
        Log                 = log;
        Configuration       = configuration;
        WindowHandler       = windowHandler;
    }

    protected override Vector2 MinSize
        => WindowSize;

    protected override Vector2 MaxSize
        => WindowSize;

    protected override Vector2 DefaultSize
        => WindowSize;

    public override void OnClose()
        => WindowHandler.AdvancedConfigurationWindow.IsOpen = false;

    public override void Draw()
    {
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Coffee.ToIconString(), new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight)))
        {
            Util.OpenLink("https://ko-fi.com/glyceri");
        }

        ImGui.PopFont();

        ImGui.SameLine();

        ImGui.BeginDisabled();

        BasicLabel.Draw("Ko-Fi for support.", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight));

        ImGui.EndDisabled();

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Plugin Enabled", ref Configuration.Enabled))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(WindowHandler.CheckboxHeight, WindowHandler.CheckboxHeight)))
        {
            if (ImGui.Checkbox("Add plugin name to chat messages.", ref Configuration.AddPluginToChat))
            {
                Configuration.Save(DalamudPlugin, Log);
            }
        }

        _ = ImGui.InvisibleButton("", new Vector2(WindowHandler.BarHeight, WindowHandler.BarHeight));

        ImGui.BeginDisabled(WindowHandler.AdvancedConfigurationWindow.IsOpen);

        if (ImGui.Button("Open Advanced Settings", new Vector2(ImGui.GetContentRegionAvail().X, WindowHandler.BarHeight)))
        {
            WindowHandler.AdvancedConfigurationWindow.IsOpen = true;
        }

        ImGui.EndDisabled();
    }
}
