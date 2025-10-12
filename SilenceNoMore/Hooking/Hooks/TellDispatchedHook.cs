using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.Shell;
using SilenceNoMore.Hooking.Constants;
using System;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TellDispatchedHook : HookableElement
{
    [Signature(Signatures.ExecuteTellCommandSignature, DetourName = nameof(ExecuteTellCommandDetour))]
    private readonly Hook<Delegates.ExecuteTellCommandDelegate>? ExecuteTellCommandHook;

    [Signature(Signatures.GetTerritoryIntendedUseSignature, DetourName = nameof(GetTerritoryIntendedUseDetour))]
    private readonly Hook<Delegates.GetTerritoryIntendedUseDelegate>? GetTerritoryIntendedUseHook;

    private bool triedToSendTell = false;

    public TellDispatchedHook(IPluginLog log, IGameInteropProvider hooker, IConfiguration configuration)
        : base(hooker, log, configuration) { }

    public override void Init()
    {
        ExecuteTellCommandHook?.Enable();
        GetTerritoryIntendedUseHook?.Enable();
    }

    private nint GetTerritoryIntendedUseDetour(uint rowIdOrIndex)
    {
        nint returner = GetTerritoryIntendedUseHook!.OriginalDisposeSafe(rowIdOrIndex);

        if (!triedToSendTell)
        {
            return returner;
        }

        if (rowIdOrIndex != 3)
        {
            return returner;
        }

        if (!Configuration.CanSendInDuty)
        {
            return returner;
        }

        Log.Verbose("Just overwrote GetTerritoryIntendedUse to allow sending the whisper.");

        return GetTerritoryIntendedUseHook!.OriginalDisposeSafe(0);
    }

    private int ExecuteTellCommandDetour(ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule)
    {
        try
        {
            Log.Verbose("Just tried to send a tell!");

            triedToSendTell = true;

            int returner = ExecuteTellCommandHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);

            triedToSendTell = false;

            return returner;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in ShellCommandChatTell_ExecuteCommandDetour.");
        }

        return ExecuteTellCommandHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);
    }

    public override void Dispose()
    {
        ExecuteTellCommandHook?.Dispose();
        GetTerritoryIntendedUseHook?.Dispose();
    }
}
