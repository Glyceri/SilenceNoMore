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

        DalamudPlugin.UiBuilder.Draw         += OnDraw;
        DalamudPlugin.UiBuilder.OpenConfigUi += () =>
        {
            ConfigurationWindow.IsOpen = true;
        };
    }

    private void OnDraw()
    {
        WindowSystem.Draw();
    }

    public void Dispose()
    {
        DalamudPlugin.UiBuilder.Draw -= OnDraw;

        WindowSystem.RemoveAllWindows();
    }
}
