using Dalamud.Plugin.Services;
using System;

namespace SilenceNoMore.Hooking;

internal abstract class HookableElement : IDisposable
{
    protected readonly IGameInteropProvider Hooker;
    protected readonly IPluginLog           Log;
    protected readonly IConfiguration       Configuration;

    public HookableElement(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration)
    {
        Hooker          = hooker;
        Log             = log;
        Configuration   = configuration;

        try
        {
            Hooker.InitializeFromAttributes(this);
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout tijdens het gereedmaken van de 'Hooks'.");
        }
    }

    public abstract void Init();
    public abstract void Dispose();
}
