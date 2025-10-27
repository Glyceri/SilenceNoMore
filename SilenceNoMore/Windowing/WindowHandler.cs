using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace SilenceNoMore.Windowing;

internal class WindowHandler : IDisposable
{
    private readonly IDalamudPluginInterface        DalamudPlugin;
    private readonly IPluginLog                     PluginLog;
    private readonly Configuration                  Configuration;
    private readonly WindowSystem                   WindowSystem;

    public readonly  ConfigurationWindow            ConfigurationWindow;
    public readonly  AdvancedConfigurationWindow    AdvancedConfigurationWindow;

    public WindowHandler(IDalamudPluginInterface dalamudPlugin, IPluginLog log, Configuration configuration)
    {
        DalamudPlugin               = dalamudPlugin;

        PluginLog                   = log;
        Configuration               = configuration;

        WindowSystem                = new WindowSystem("Silence No More");

        AdvancedConfigurationWindow = new AdvancedConfigurationWindow(DalamudPlugin, PluginLog, Configuration, this);
        ConfigurationWindow         = new ConfigurationWindow(DalamudPlugin, PluginLog, Configuration, this);

        WindowSystem.AddWindow(ConfigurationWindow);
        WindowSystem.AddWindow(AdvancedConfigurationWindow);

        DalamudPlugin.UiBuilder.Draw         += WindowSystem.Draw;
        DalamudPlugin.UiBuilder.OpenConfigUi += () =>
        {
            ConfigurationWindow.IsOpen = true;
        };
    }

    // The 16 is because this plugin was made for exlusively dalamud font size 12 (which is font scale 16 in ImGUI).
    // Scaling the whole UI thingy around it seems to work perfectly fine
    public static float FontScale
        => (ImGui.GetFontSize() / 16.0f);

    public static float GlobalScale
        => ImGuiHelpers.GlobalScale * FontScale;

    private static float RawBarHeight
        => 30;

    public static float BarHeight
        => RawBarHeight * GlobalScale;

    public static float CheckboxHeight 
        => (BarHeight - ImGui.GetTextLineHeight()) / 2;

    public void Dispose()
    {
        DalamudPlugin.UiBuilder.Draw -= WindowSystem.Draw;

        WindowSystem.RemoveAllWindows();
    }
}
