using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using SilenceNoMore.Hooking.Constants;
using SilenceNoMore.Hooking.Structs;
using System;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class TellReceivedHook : HookableElement
{
    [Signature(Signatures.IsAllowedToReceiveDirectMessagesSignature, DetourName = nameof(IsAllowedToReceiveDirectMessagesDetour))]
    private readonly Hook<Delegates.IsAllowedToReceiveDirectMessagesDelegate>? IsAllowedToReceiveDirectMessagesHook;

    [Signature(Signatures.OnNetworkChatSignature, DetourName = nameof(OnNetworkChatDetour))]
    private readonly Hook<Delegates.OnNetworkChatDelegate>? OnNetworkChatHook;

    [Signature(Signatures.MessageBlockedSignature, DetourName = nameof(MessageBlockedDetour))]
    private readonly Hook<Delegates.MessageBlockedDelegate>? MessageBlockedHook;

    private bool isValidCheck      = false;
    private bool manuallyOverwrote = false;

    public TellReceivedHook(IPluginLog log, IGameInteropProvider hooker, IConfiguration configuration)
        : base(hooker, log, configuration) { }

    public override void Init()
    {
        IsAllowedToReceiveDirectMessagesHook?.Enable();
        OnNetworkChatHook?.Enable();
    }

    private byte IsAllowedToReceiveDirectMessagesDetour(RaptureShellModule* a1, int checkType, byte a3, byte a4)
    {
        Log.Verbose("Just received a whisper.");

        byte returnValue = IsAllowedToReceiveDirectMessagesHook!.OriginalDisposeSafe(a1, checkType, a3, a4);

        if (!isValidCheck)
        {
            return returnValue;
        }

        if (!Configuration.IsEnabled)
        {
            Log.Verbose("Tried to relay aber Configuration.IsEnabled is false.");

            return returnValue;
        }

        if (returnValue != 0)
        {
            Log.Verbose("Tried to relay but returnValue is not 0.");

            return returnValue;
        }

        if (checkType != 2)
        {
            Log.Verbose("Tried to relay but checkType is not 2.");

            return returnValue;
        }

        Log.Verbose("Manually overwrote the whisper.");

        manuallyOverwrote = true;

        return 1;
    }

    private void OnNetworkChatDetour(nint a1, MessagePacket* messagePacket)
    {
        isValidCheck = true;

        try
        {
            Log.Verbose($"Just received a whisper from:" +
                $"{Environment.NewLine}[{messagePacket->SenderContentId}]" +
                $"{Environment.NewLine}[{messagePacket->MessageType}]");
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in OnNetworkChatDetour.");
        }

        try
        {
            OnNetworkChatHook?.OriginalDisposeSafe(a1, messagePacket);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in OnNetworkChatHook.");
        }

        Log.Verbose($"manuallyOverwrote: {manuallyOverwrote} && Configuration.CanReturnError: {Configuration.CanReturnError}");

        // This means we forced the message to display on our client.
        // Let's tell the server we didn't see it though.
        if (manuallyOverwrote && Configuration.CanReturnError)
        {
            SendMessageFailed(messagePacket->SenderContentId);
        }

        isValidCheck      = false;
        manuallyOverwrote = false;
    }

    private void SendMessageFailed(ulong contentId)
    {
        if (!isValidCheck)
        {
            Log.Verbose($"failed isValidCheck.");

            return;
        }

        try
        {
            if (Framework.Instance()->NetworkModuleProxy == null)
            {
                Log.Debug("Sending message failed because NetworkModuleProxy is NULL.");

                return;
            }

            if (MessageBlockedDetour(Framework.Instance()->NetworkModuleProxy, contentId) != 1)
            {
                Log.Debug("Calling MessageBlockedDetour failed internally... well, nothign we can do about it now c:");
            }

            Log.Verbose($"Returned block message.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in SendMessageFailed.");
        }
    }

    private char MessageBlockedDetour(NetworkModuleProxy* networkModuleProxy, ulong contentId)
    {
        Log.Verbose("Handle message blocked: " + contentId);

        return MessageBlockedHook!.OriginalDisposeSafe(networkModuleProxy, contentId);
    }

    public override void Dispose()
    {
        IsAllowedToReceiveDirectMessagesHook?.Dispose();
        OnNetworkChatHook?.Dispose();
        MessageBlockedHook?.Dispose();
    }
}
