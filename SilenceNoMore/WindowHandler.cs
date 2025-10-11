using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace SilenceNoMore;

internal class WindowHandler : IDisposable
{
    private readonly IDalamudPluginInterface DalamudPlugin;
    private readonly IPluginLog              PluginLog;
    private readonly Configuration           Configuration;
    private readonly WindowSystem            WindowSystem;

    private readonly ConfigurationWindow     ConfigurationWindow;

    public WindowHandler(IDalamudPluginInterface dalamudPlugin, IPluginLog log, Configuration configuration)
    {
        DalamudPlugin       = dalamudPlugin;

        PluginLog           = log;
        Configuration       = configuration;

        WindowSystem        = new WindowSystem("Silence No More");

        ConfigurationWindow = new ConfigurationWindow(dalamudPlugin, PluginLog, Configuration);

        WindowSystem.AddWindow(ConfigurationWindow);

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
