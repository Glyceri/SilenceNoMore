using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TellHandlerHook : HookableElement
{
    private readonly Hook<RaptureShellModule.Delegates.SetContextTellTarget>?        SetContextTellTargetHook        = null!;
    private readonly Hook<RaptureShellModule.Delegates.SetContextTellTargetInForay>? SetContextTellTargetInForayHook = null!;
    private readonly Hook<RaptureShellModule.Delegates.SetTellTargetInForay>?        SetTellTargetInForayHook        = null!;
    private readonly Hook<RaptureShellModule.Delegates.ReplyInSelectedChatMode>?     ReplyInSeletedChatModeHook      = null!;

    public TellHandlerHook(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration) 
        : base(hooker, log, configuration)
    {
        SetContextTellTargetHook        = Hooker.HookFromAddress<RaptureShellModule.Delegates.SetContextTellTarget>(RaptureShellModule.MemberFunctionPointers.SetContextTellTarget, SetContextTellTargetDetour);
        SetContextTellTargetInForayHook = Hooker.HookFromAddress<RaptureShellModule.Delegates.SetContextTellTargetInForay>(RaptureShellModule.MemberFunctionPointers.SetContextTellTargetInForay, SetContextTellTargetInForayDetour);
        SetTellTargetInForayHook        = Hooker.HookFromAddress<RaptureShellModule.Delegates.SetTellTargetInForay>(RaptureShellModule.MemberFunctionPointers.SetTellTargetInForay, SetTellTargetInForayDetour);
        ReplyInSeletedChatModeHook      = Hooker.HookFromAddress<RaptureShellModule.Delegates.ReplyInSelectedChatMode>(RaptureShellModule.MemberFunctionPointers.ReplyInSelectedChatMode, ReplyInSelectedChatModeDetour);
    }

    public override void Init()
    {
        SetContextTellTargetHook?.Enable();
        SetContextTellTargetInForayHook?.Enable();
        SetTellTargetInForayHook?.Enable();
        ReplyInSeletedChatModeHook?.Enable();
    }

    private bool SetContextTellTargetDetour(RaptureShellModule* thisPtr, Utf8String* playerName, Utf8String* worldName, ushort worldId, ulong accountId, ulong contentId, ushort reason, bool setChatType)
    {
        Log.Verbose($"Heeft Context Tell Target aangeroepen: {playerName->ToString()}@{worldName->ToString()} {accountId} {contentId}");

        return SetContextTellTargetHook!.OriginalDisposeSafe(thisPtr, playerName, worldName, worldId, accountId, contentId, reason, setChatType);
    }

    private void SetContextTellTargetInForayDetour(RaptureShellModule* thisPtr, Utf8String* playerName, Utf8String* worldName, ushort worldId, ulong accountId, ulong contentId, ushort reason)
    {
        Log.Verbose($"Heeft Context Tell Target In Foray aangeroepen: {playerName->ToString()}@{worldName->ToString()} {accountId} {contentId}");

        SetContextTellTargetInForayHook!.OriginalDisposeSafe(thisPtr, playerName, worldName, worldId, accountId, contentId, reason);
    }

    private bool SetTellTargetInForayDetour(RaptureShellModule* thisPtr, Utf8String* playerName, Utf8String* worldName, ushort worldId, ulong accountId, ulong contentId, ushort reason, bool setChatType)
    {
        Log.Verbose($"Heeft Tell Target In Foray aangeroepen: {playerName->ToString()}@{worldName->ToString()} {accountId} {contentId}");

        return SetTellTargetInForayHook!.OriginalDisposeSafe(thisPtr, playerName, worldName, worldId, accountId, contentId, reason, setChatType);
    }

    private void ReplyInSelectedChatModeDetour(RaptureShellModule* thisPtr)
    {
        Log.Verbose("Antwoord in geselecteerde 'chat' modus.");

        ReplyInSeletedChatModeHook!.OriginalDisposeSafe(thisPtr);
    }

    public override void Dispose()
    {
        SetContextTellTargetHook?.Dispose();
        SetContextTellTargetInForayHook?.Dispose();
        SetTellTargetInForayHook?.Dispose();
        ReplyInSeletedChatModeHook?.Dispose();
    }
}
