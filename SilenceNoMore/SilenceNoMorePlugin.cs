using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace SilenceNoMore;

public sealed unsafe class SilenceNoMorePlugin : IDalamudPlugin
{
    private readonly Configuration Configuration;
    private readonly Hooks         Hooks;
    private readonly TellHook      TellHook;
    private readonly WindowHandler WindowHandler;

    [PluginService] internal IPluginLog           Log       { get; private set; } = null!;
    [PluginService] internal IGameInteropProvider Hooker    { get; private set; } = null!;

    public SilenceNoMorePlugin(IDalamudPluginInterface plugin)
    {
        Configuration = plugin.GetPluginConfig() as Configuration ?? new Configuration();

        Hooks         = new Hooks(Log, Hooker, Configuration);
        TellHook      = new TellHook(Log, Hooker, Configuration);

        WindowHandler = new WindowHandler(plugin, Log, Configuration);
    }

    public void Dispose()
    {
        try
        {
            Hooks?.Dispose();
            TellHook?.Dispose();
        }
        catch (Exception e) 
        {
            Log.Error(e, "Failure in disposing Hooks.");
        }

        try
        {
            WindowHandler?.Dispose();
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in disposing WindowHandler.");
        }
    }
}
