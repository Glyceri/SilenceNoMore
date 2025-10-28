using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.Shell;
using SilenceNoMore.Hooking.Constants;
using SilenceNoMore.Hooking.Enums;
using SilenceNoMore.Hooking.Structs;
using SilenceNoMore.TellHandling;
using SilenceNoMore.TellHandling.Enum;
using System;
using System.Runtime.InteropServices;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TellDispatchedHook : HookableElement
{
    private readonly TellHandler TellHandler;

    [Signature(Signatures.ExecuteTellCommandSignature, DetourName = nameof(ExecuteTellCommandDetour))]
    private readonly Hook<Delegates.ExecuteTellCommandDelegate>? ExecuteTellCommandHook;

    [Signature(Signatures.SendPublicTellSignature, DetourName = nameof(SendPublicTellDetour))]
    private readonly Hook<Delegates.SendPublicTellDelegate>? SendPublicTellHook;
    
    private bool               triedToSendTell    = false;
    private ChatCallStagesEnum currentCheckIndex  = ChatCallStagesEnum.None;
    private bool               cameFromSendTell   = false;

    private static TerritoryChatRule _territoryChatRule;
    private static GCHandle          _territoryChatRuleHandle;

    public TellDispatchedHook(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration, TellHandler tellHandler)
        : base(hooker, log, configuration)
    {
        TellHandler = tellHandler;
    }

    public override void Init()
    {
        _territoryChatRuleHandle = GCHandle.Alloc(_territoryChatRule, GCHandleType.Pinned);

        ExecuteTellCommandHook?.Enable();
        SendPublicTellHook?.Enable();
    }

    private byte SendPublicTellDetour(nint a1, nint a2, ushort a3, nint a4, nint a5, byte tellReason)
    {
        Log.Verbose("Verstuur een openbaar fluisterbericht.");

        return SendPublicTellHook!.OriginalDisposeSafe(a1, a2, a3, a4, a5, tellReason);
    }

    public TerritoryIntendedUse* GetTerritoryIntendedUseDetour(TerritoryIntendedUse* returner, TerritoryIntendedUseEnum rowIdOrIndex)
    {
        if (!triedToSendTell)
        {
            return returner;
        }

        if (!Configuration.CanSendInDuty)
        {
            Log.Verbose("'GetTerritoryIntendedUseDetour': De instelling 'CanSendInDuty' is uitgeschakeld.");

            return returner;
        }

        if (!Settings.CurrentTerritoryIsAllowed())
        {
            Log.Verbose("'GetTerritoryIntendedUseDetour': Het huidige territorium is NIET toegestaan.");

            return returner;
        }

        currentCheckIndex++;

        cameFromSendTell = true;

        return returner;
    }

    public TerritoryChatRule* GetTerritoryChatRuleDetour(TerritoryChatRule* returner, TerritoryChatRuleEnum rowIdOrIndex)
    {
        if (!cameFromSendTell)
        {
            return returner;
        }

        Log.Verbose($"ONTVANGEN:{Environment.NewLine}" +
                    $"Current State: [{currentCheckIndex}].{Environment.NewLine}" +
                    $"Row Id or Index: [{rowIdOrIndex}].{Environment.NewLine}" +
                    $"Duty Tell State: [{returner->DutyTell}].{Environment.NewLine}" +
                    $"Public Tell State: [{returner->PublicTell}].");

        if (returner->PublicTell != ChatBlockStateEnum.Restricted)
        {
            Log.Verbose("'GetTerritoryIntendedUseDetour': 'Public tell' is NIET beperkt.");

            return returner;
        }

        cameFromSendTell = false;

        if (!Configuration.CanSendInDuty)
        {
            Log.Verbose("'GetTerritoryIntendedUseDetour': De instelling 'CanSendInDuty' is uitgeschakeld.");

            return returner;
        }

        if (!Settings.ChatRuleCanBeOverriden(rowIdOrIndex))
        {
            Log.Verbose("'GetTerritoryIntendedUseDetour': 'ChatRule' kan NIET overschreven worden.");

            return returner;
        }

        bool dutyRestricted = returner->DutyTell == ChatBlockStateEnum.Restricted;

        if (!dutyRestricted && TellHandler.TellState == TellState.DutyTell)
        {
            return returner;
        }

        TerritoryChatRule* territoryObject = (TerritoryChatRule*)_territoryChatRuleHandle.AddrOfPinnedObject();

        territoryObject->Public     = returner->Public;
        territoryObject->Shout      = returner->Shout;
        territoryObject->DutyTell   = returner->DutyTell;
        territoryObject->Party      = returner->Party;
        territoryObject->Global     = returner->Global;
        territoryObject->Pvp        = returner->Pvp;
        territoryObject->UnkBool    = returner->UnkBool;

        territoryObject->DutyTell   = ChatBlockStateEnum.Restricted;
        territoryObject->PublicTell = ChatBlockStateEnum.NoRestriction;

        Log.Verbose($"Heeft zojuist de 'GetTerritoryChatRuleDetour' overschreven om het versturen van een fluisterbericht toe te staan.{Environment.NewLine}{territoryObject->ToString()}");

        return territoryObject;
    }

    private int ExecuteTellCommandDetour(ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule)
    {
        try
        {
            Log.Verbose("Zojuist is er geprobeerd een fluisterbericht te sturen!");

            triedToSendTell   = true;
            currentCheckIndex = 0;

            int returner = ExecuteTellCommandHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);

            triedToSendTell   = false;
            currentCheckIndex = 0;

            return returner;
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout in 'ShellCommandChatTell_ExecuteCommandDetour'.");
        }

        return ExecuteTellCommandHook!.OriginalDisposeSafe(shellCommands, tell, uiModule);
    }

    public override void Dispose()
    {
        _territoryChatRuleHandle.Free();

        SendPublicTellHook?.Dispose();
        ExecuteTellCommandHook?.Dispose();
    }
}
