using Dalamud.Plugin.Services;
using SilenceNoMore.Hooking.Hooks;
using SilenceNoMore.TellHandling;
using System;
using System.Collections.Generic;

namespace SilenceNoMore.Hooking;

internal class HookHandler : IDisposable
{
    private readonly IPluginLog            Log;
    private readonly IAddonLifecycle       AddonLifecycle;
    private readonly TellHandler           TellHandler;
    private readonly IClientState          ClientState;
    private readonly List<HookableElement> Hooks = [];

    private readonly TellDispatchedHook TellDispatchedHook;
    private readonly TellReceivedHook   TellReceivedHook;
    private readonly TellHandlerHook    TellHandlerHook;
    private readonly ChatLogHook        ChatLogHook;
    private readonly TerritoryHook      TerritoryHook;

    public HookHandler(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration, IAddonLifecycle addonLifecycle, TellHandler tellHandler, IClientState clientState)
    {
        Log            = log;
        TellHandler    = tellHandler;
        ClientState    = clientState;
        AddonLifecycle = addonLifecycle;

        RegisterHook(TellDispatchedHook = new TellDispatchedHook(hooker, log, configuration, TellHandler));
        RegisterHook(TellReceivedHook   = new TellReceivedHook(hooker, log, configuration));
        RegisterHook(TellHandlerHook    = new TellHandlerHook(hooker, log, configuration));
        RegisterHook(ChatLogHook        = new ChatLogHook(hooker, log, configuration, AddonLifecycle, TellHandler, TellHandlerHook));
        RegisterHook(TerritoryHook      = new TerritoryHook(hooker, log, configuration, ClientState, TellDispatchedHook, TellHandler));
    }

    private void RegisterHook(HookableElement hookableElement)
    {
        _ = Hooks.Remove(hookableElement);

        Hooks.Add(hookableElement);
    }

    public void Initialize()
    {
        foreach (HookableElement hookableElement in Hooks)
        {
            hookableElement.Init();
        }
    }

    public void Dispose()
    {
        foreach (HookableElement hookableElement in Hooks)
        {
            try
            {
                hookableElement?.Dispose();
            }
            catch(Exception e)
            {
                Log.Error(e, $"De 'Hook' [{hookableElement.GetType().Name}] is niet volledig opgeruimd.");
            }
        }
    }
}
