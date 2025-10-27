using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using SilenceNoMore.Hooking.Constants;
using SilenceNoMore.Hooking.Enums;
using SilenceNoMore.Hooking.Structs;
using SilenceNoMore.TellHandling;
using SilenceNoMore.TellHandling.Enum;
using System;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TerritoryHook : HookableElement
{
    private readonly IClientState       ClientState;
    private readonly TellDispatchedHook TellDispatchedHook;
    private readonly TellHandler        TellHandler;

    private ushort                   _currentTerritory              = 0;
    private TerritoryIntendedUseEnum _currentTerritoryIntendedUseId = 0;

    [Signature(Signatures.GetTerritoryIntendedUseSignature, DetourName = nameof(GetTerritoryIntendedUseDetour))]
    private readonly Hook<Delegates.GetTerritoryIntendedUseDelegate>? GetTerritoryIntendedUseHook;

    [Signature(Signatures.GetTerritoryChatRuleSignature, DetourName = nameof(GetTerritoryChatRuleDetour))]
    private readonly Hook<Delegates.GetTerritoryChatRuleDelegate>? GetTerritoryChatRuleHook;

    public TerritoryHook(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration, IClientState clientState, TellDispatchedHook tellDispatchedHook, TellHandler tellHandler) 
        : base(hooker, log, configuration)
    {
        ClientState        = clientState;
        TellDispatchedHook = tellDispatchedHook;
        TellHandler        = tellHandler;

        ClientState.TerritoryChanged += OnTerritoryChangedDetour;
    }

    public override void Init()
    {
        SetTerritoryId(ClientState.TerritoryType);

        GetTerritoryIntendedUseHook?.Enable();
        GetTerritoryChatRuleHook?.Enable();
    }

    private void OnTerritoryChangedDetour(ushort territoryId)
    {
        SetTerritoryId(territoryId);
    }

    private TerritoryIntendedUse* GetTerritoryIntendedUseDetour(TerritoryIntendedUseEnum rowIdOrIndex)
    {
        TerritoryIntendedUse* returner = OriginalGetTerritoryIntendedUse(rowIdOrIndex);

        return TellDispatchedHook.GetTerritoryIntendedUseDetour(returner, rowIdOrIndex);
    }

    private TerritoryChatRule* GetTerritoryChatRuleDetour(TerritoryChatRuleEnum rowIdOrIndex)
    {
        TerritoryChatRule* returner = OriginalGetTerritoryChatRule(rowIdOrIndex);

        return TellDispatchedHook.GetTerritoryChatRuleDetour(returner, rowIdOrIndex);
    }

    private TerritoryIntendedUse* OriginalGetTerritoryIntendedUse(TerritoryIntendedUseEnum rowIdOrIndex)
        => GetTerritoryIntendedUseHook!.OriginalDisposeSafe(rowIdOrIndex);

    private TerritoryChatRule* OriginalGetTerritoryChatRule(TerritoryChatRuleEnum rowIdOrIndex)
        => GetTerritoryChatRuleHook!.OriginalDisposeSafe(rowIdOrIndex);

    private void SetTerritoryId(ushort territoryId)
    {
        _currentTerritory              = territoryId;
        _currentTerritoryIntendedUseId = (TerritoryIntendedUseEnum)GameMain.Instance()->CurrentTerritoryIntendedUseId;

        Log.Verbose($"Huidige territorium is veranderd naar: {_currentTerritory} {_currentTerritoryIntendedUseId}");

        try
        {
            TerritoryIntendedUse* territoryIntendedUse = OriginalGetTerritoryIntendedUse(_currentTerritoryIntendedUseId);

            if (territoryIntendedUse == null)
            {
                return;
            }

            TerritoryChatRule* territoryChatRule = OriginalGetTerritoryChatRule(territoryIntendedUse->TerritoryChatRule);

            if (territoryChatRule == null)
            {
                return;
            }

            Log.Verbose("Huidige territorium heeft de 'ChatRule': " + territoryChatRule->ToString());

            bool canDutyTell = (territoryChatRule->DutyTell == ChatBlockStateEnum.NoRestriction);

            TellHandler.SetDutyTellRestriction(canDutyTell ? DutyTellRestriction.NoRestriction : DutyTellRestriction.Restricted);

            if (Configuration.ShouldAutoSwitchChatMode)
            {
                TellHandler.SetTellState(canDutyTell ? TellState.DutyTell : TellState.GlobalTell);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fout tijdens het ophalen van de 'Chat Rule' van de huidige 'Territory Id'.");
        }
    }

    public override void Dispose()
    {
        ClientState.TerritoryChanged -= OnTerritoryChangedDetour;

        GetTerritoryIntendedUseHook?.Dispose();
        GetTerritoryChatRuleHook?.Dispose();
    }
}
