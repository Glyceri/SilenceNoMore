using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.Shell;
using SilenceNoMore.Hooking.Constants;
using SilenceNoMore.Hooking.Enums;
using SilenceNoMore.Hooking.Structs;
using System;
using System.Runtime.InteropServices;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TellDispatchedHook : HookableElement
{
    [Signature(Signatures.ExecuteTellCommandSignature, DetourName = nameof(ExecuteTellCommandDetour))]
    private readonly Hook<Delegates.ExecuteTellCommandDelegate>? ExecuteTellCommandHook;

    [Signature(Signatures.GetTerritoryIntendedUseSignature, DetourName = nameof(GetTerritoryIntendedUseDetour))]
    private readonly Hook<Delegates.GetTerritoryIntendedUseDelegate>? GetTerritoryIntendedUseHook;

    [Signature(Signatures.GetTerritoryChatRuleSignature, DetourName = nameof(GetTerritoryChatRuleDetour))]
    private readonly Hook<Delegates.GetTerritoryChatRuleDelegate>? GetTerritoryChatRuleHook;

    [Signature(Signatures.SendPublicTellSignature, DetourName = nameof(SendPublicTellDetour))]
    private readonly Hook<Delegates.SendPublicTellDelegate>? SendPublicTellHook;
    
    private bool               triedToSendTell    = false;
    private ChatCallStagesEnum currentCheckIndex  = ChatCallStagesEnum.None;
    private bool               cameFromSendTell   = false;

    private static TerritoryChatRule _territoryChatRule;
    private static GCHandle          _territoryChatRuleHandle;

    public TellDispatchedHook(IPluginLog log, IGameInteropProvider hooker, IConfiguration configuration)
        : base(hooker, log, configuration) { }

    public override void Init()
    {
        _territoryChatRuleHandle = GCHandle.Alloc(_territoryChatRule, GCHandleType.Pinned);

        ExecuteTellCommandHook?.Enable();
        GetTerritoryIntendedUseHook?.Enable();
        GetTerritoryChatRuleHook?.Enable();
        SendPublicTellHook?.Enable();
    }

    private byte SendPublicTellDetour(nint a1, nint a2, ushort a3, nint a4, nint a5, byte tellReason)
    {
        Log.Verbose("Send public tell.");

        return SendPublicTellHook!.OriginalDisposeSafe(a1, a2, a3, a4, a5, tellReason);
    }

    private nint GetTerritoryIntendedUseDetour(uint rowIdOrIndex)
    {
        nint returner = GetTerritoryIntendedUseHook!.OriginalDisposeSafe(rowIdOrIndex);

        if (!triedToSendTell)
        {
            return returner;
        }

        if (!Configuration.CanSendInDuty)
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: Setting CanSendInDuty is disabled.");

            return returner;
        }

        if (!Settings.CurrentTerritoryIsAllowed())
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: Current territory is NOT allowed.");

            return returner;
        }

        currentCheckIndex++;

        cameFromSendTell = true;

        return returner;
    }


    private TerritoryChatRule* GetTerritoryChatRuleDetour(TerritoryChatRuleEnum rowIdOrIndex)
    {
        TerritoryChatRule* returner = GetTerritoryChatRuleHook!.OriginalDisposeSafe(rowIdOrIndex);

        if (!cameFromSendTell)
        {
            return returner;
        }

        Log.Verbose($"RECEIVED:{Environment.NewLine}" +
                    $"Current State: [{currentCheckIndex}].{Environment.NewLine}" +
                    $"Row Id or Index: [{rowIdOrIndex}].{Environment.NewLine}" +
                    $"Duty Tell State: [{returner->DutyTell}].{Environment.NewLine}" +
                    $"Public Tell State: [{returner->PublicTell}].");

        if (returner->PublicTell != ChatBlockStateEnum.Restricted)
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: Public tell is NOT restricted.");

            return returner;
        }

        cameFromSendTell = false;

        if (!Configuration.CanSendInDuty)
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: Setting CanSendInDuty is disabled.");

            return returner;
        }

        if (!Settings.ChatRuleCanBeOverriden(rowIdOrIndex))
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: ChatRule can NOT be overriden.");

            return returner;
        }

        bool dutyRestricted = returner->DutyTell == ChatBlockStateEnum.Restricted;

        if (!dutyRestricted)
        {
            Log.Verbose("GetTerritoryIntendedUseDetour: This needs some work still.");

            return returner;
        }

        TerritoryChatRule* territoryObject = (TerritoryChatRule*)_territoryChatRuleHandle.AddrOfPinnedObject();

        territoryObject->Public         = returner->Public;
        territoryObject->Shout          = returner->Shout;
        territoryObject->DutyTell       = returner->DutyTell;
        territoryObject->Party          = returner->Party;
        territoryObject->Global         = returner->Global;
        territoryObject->Pvp            = returner->Pvp;
        territoryObject->UnkBool        = returner->UnkBool;

        territoryObject->DutyTell       = ChatBlockStateEnum.Restricted;
        territoryObject->PublicTell     = ChatBlockStateEnum.NoRestriction;

        Log.Verbose($"Just overwrote GetTerritoryChatRuleDetour to allow sending the whisper.{Environment.NewLine}" +
                    $"Public: [{territoryObject->Public}]{Environment.NewLine}" +
                    $"Shout: [{territoryObject->Shout}]{Environment.NewLine}" +
                    $"DutyTell: [{territoryObject->DutyTell}]{Environment.NewLine}" +
                    $"PublicTell: [{territoryObject->PublicTell}]{Environment.NewLine}" +
                    $"Party: [{territoryObject->Party}]{Environment.NewLine}" +
                    $"Global: [{territoryObject->Global}]{Environment.NewLine}" +
                    $"Pvp: [{territoryObject->Pvp}]{Environment.NewLine}");

        return territoryObject;
    }

    private int ExecuteTellCommandDetour(ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule)
    {
        try
        {
            Log.Verbose("Just tried to send a tell!");

            triedToSendTell   = true;
            currentCheckIndex = 0;

            int returner = ExecuteTellCommandHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);

            triedToSendTell   = false;
            currentCheckIndex = 0;

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
        _territoryChatRuleHandle.Free();

        SendPublicTellHook?.Dispose();
        ExecuteTellCommandHook?.Dispose();
        GetTerritoryIntendedUseHook?.Dispose();
        GetTerritoryChatRuleHook?.Dispose();
    }
}
