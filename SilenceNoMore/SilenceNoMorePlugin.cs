using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using SilenceNoMore.Commands;
using SilenceNoMore.Hooking;
using SilenceNoMore.Hooking.Enums;
using SilenceNoMore.Windowing;
using System;

namespace SilenceNoMore;

public sealed unsafe class SilenceNoMorePlugin : IDalamudPlugin
{
    private readonly Configuration      Configuration;
    private readonly HookHandler        HookHandler;
    private readonly WindowHandler      WindowHandler;
    private readonly CommandHandler     CommandHandler;

    [PluginService] internal IPluginLog           Log            { get; private set; } = null!;
    [PluginService] internal IGameInteropProvider Hooker         { get; private set; } = null!;
    [PluginService] internal ICommandManager      CommandManager { get; private set; } = null!;
    [PluginService] internal IChatGui             ChatGui        { get; private set; } = null!;
    [PluginService] internal IClientState         ClientState    { get; private set; } = null!;

    public SilenceNoMorePlugin(IDalamudPluginInterface plugin)
    {
        Configuration   = plugin.GetPluginConfig() as Configuration ?? new Configuration();

        HookHandler     = new HookHandler(Hooker, Log, Configuration);

        WindowHandler   = new WindowHandler(plugin, Log, Configuration);

        CommandHandler  = new CommandHandler(CommandManager, WindowHandler, ChatGui);
    }

    public void Dispose()
    {
        try
        {
            CommandHandler?.Dispose();
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in disposing CommandHandler.");
        }

        try
        {
            HookHandler?.Dispose();
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
