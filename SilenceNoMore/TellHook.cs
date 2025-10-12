using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.Shell;
using System;

namespace SilenceNoMore;

internal unsafe class TellHook : IDisposable
{
    private readonly Configuration        Configuration;
    private readonly IPluginLog           Log;
    private readonly IGameInteropProvider Hooker;

    private delegate int ShellCommandChatTell_ExecuteCommandDelegate(ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule);
    private delegate nint GetTerritoryIntendedUseDelegate(uint rowIdOrIndex);

    [Signature("40 55 53 56 57 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? B8", DetourName = nameof(ShellCommandChatTell_ExecuteCommandDetour))]
    private readonly Hook<ShellCommandChatTell_ExecuteCommandDelegate>? ExecuteTellHook;

    [Signature("E8 ?? ?? ?? ?? 8B BE ?? ?? ?? ?? 4C 8B E0", DetourName = nameof(GetTerritoryIntendedUseDetour))]
    private readonly Hook<GetTerritoryIntendedUseDelegate>? GetTerritoryIntendedUseHook;

    private bool triedToSendTell = false;

    public TellHook(IPluginLog log, IGameInteropProvider hooker, Configuration configuration)
    {
        Log           = log;
        Hooker        = hooker;
        Configuration = configuration;

        try
        {
            Hooker.InitializeFromAttributes(this);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in constructor.");
        }

        ExecuteTellHook?.Enable();
        GetTerritoryIntendedUseHook?.Enable();
    }

    private nint GetTerritoryIntendedUseDetour(uint rowIdOrIndex)
    {
        if (triedToSendTell && rowIdOrIndex == 3 && Configuration.Enabled)
        {
            return GetTerritoryIntendedUseHook!.OriginalDisposeSafe(0);
        }

        return GetTerritoryIntendedUseHook!.OriginalDisposeSafe(rowIdOrIndex);
    }

    private int ShellCommandChatTell_ExecuteCommandDetour(ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule)
    {
        try
        {
            Log.Verbose("Just tried to send a tell!");

            triedToSendTell = true;

            int returner = ExecuteTellHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);

            triedToSendTell = false;

            return returner;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in ShellCommandChatTell_ExecuteCommandDetour.");
        }

        return ExecuteTellHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);
    }

    public void Dispose()
    {
        ExecuteTellHook?.Dispose();
        GetTerritoryIntendedUseHook?.Dispose();
    }
}
