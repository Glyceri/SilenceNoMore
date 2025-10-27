using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using SilenceNoMore.Chat;
using SilenceNoMore.Commands;
using SilenceNoMore.Hooking;
using SilenceNoMore.TellHandling;
using SilenceNoMore.TellHandling.Enum;
using SilenceNoMore.Windowing;
using System;

namespace SilenceNoMore;

public sealed unsafe class SilenceNoMorePlugin : IDalamudPlugin
{
    private readonly Configuration      Configuration;
    private readonly TellHandler        TellHandler;
    private readonly ChatHandler        ChatHandler;
    private readonly HookHandler        HookHandler;
    private readonly WindowHandler      WindowHandler;
    private readonly CommandHandler     CommandHandler;

    [PluginService] internal IPluginLog           Log            { get; private set; } = null!;
    [PluginService] internal IGameInteropProvider Hooker         { get; private set; } = null!;
    [PluginService] internal ICommandManager      CommandManager { get; private set; } = null!;
    [PluginService] internal IChatGui             ChatGui        { get; private set; } = null!;
    [PluginService] internal IClientState         ClientState    { get; private set; } = null!;
    [PluginService] internal IAddonLifecycle      AddonLifecycle { get; private set; } = null!;

    public SilenceNoMorePlugin(IDalamudPluginInterface plugin)
    {
        Configuration   = plugin.GetPluginConfig() as Configuration ?? new Configuration();

        ChatHandler     = new ChatHandler(ChatGui, Configuration);

        TellHandler     = new TellHandler(ChatHandler, Configuration);

        HookHandler     = new HookHandler(Hooker, Log, Configuration, AddonLifecycle, TellHandler, ClientState);

        WindowHandler   = new WindowHandler(plugin, Log, Configuration);

        CommandHandler  = new CommandHandler(CommandManager, WindowHandler, ChatHandler, TellHandler);

        HookHandler.Initialize();

        if (TellHandler.TellState == TellState.INVALID)
        {
            TellHandler.SetTellState(TellState.GlobalTell);
        }

    }

    public void Dispose()
    {
        try
        {
            CommandHandler?.Dispose();
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout tijdens het opruimen van de 'CommandHandler'.");
        }

        try
        {
            HookHandler?.Dispose();
        }
        catch (Exception e) 
        {
            Log.Error(e, "Fout tijdens het opruimen van de 'Hooks'.");
        }

        try
        {
            WindowHandler?.Dispose();
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout tijdens het opruimen van de 'WindowHandler'.");
        }
    }
}
