using Dalamud.Plugin.Services;
using SilenceNoMore.Hooking.Hooks;
using System;
using System.Collections.Generic;

namespace SilenceNoMore.Hooking;

internal class HookHandler : IDisposable
{
    private readonly IPluginLog            Log;
    private readonly List<HookableElement> Hooks = [];

    public HookHandler(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration)
    {
        Log = log;

        RegisterHook(new TellDispatchedHook(log, hooker, configuration));
        RegisterHook(new TellReceivedHook(log, hooker, configuration));

        Initialize();
    }

    private void RegisterHook(HookableElement hookableElement)
    {
        _ = Hooks.Remove(hookableElement);

        Hooks.Add(hookableElement);
    }

    private void Initialize()
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
                Log.Error(e, $"Hook [{hookableElement.GetType().Name}] failed to dipose properly.");
            }
        }
    }
}
