using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;

namespace SilenceNoMore;

internal unsafe class Hooks : IDisposable
{
    private readonly Configuration        Configuration;
    private readonly IPluginLog           Log;
    private readonly IGameInteropProvider Hooker;

    private delegate byte IsAllowedToReceiveDirectMessagesDelegate(nint a1, int a2, byte a3, byte a4);
    private delegate void OnNetworkChatDelegate(nint a1, MessagePacket* messagePacket);
    private delegate char MessageBlockedDelegate(NetworkModuleProxy* networkModuleProxy, ulong accountId);

    [Signature("48 89 5C 24 ?? 57 48 83 EC 20 48 63 FA 41 0F B6 D8", DetourName = nameof(IsAllowedToReceiveDirectMessagesDetour))]
    private readonly Hook<IsAllowedToReceiveDirectMessagesDelegate>? IsAllowedToReceiveDirectMessagesHook;

    [Signature("41 54 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 4C 8B E2", DetourName = nameof(OnNetworkChatDetour))]
    private readonly Hook<OnNetworkChatDelegate>? OnNetworkChatHook;

    [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 89 B4 24 ?? ?? ?? ?? 33 F6 41 80 7C 24", DetourName = nameof(MessageBlockedDetour))]
    private readonly Hook<MessageBlockedDelegate>? MessageBlockedHook;

    private bool manuallyOverwrote = false;

    public Hooks(IPluginLog log, IGameInteropProvider hooker, Configuration configuration)
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

        IsAllowedToReceiveDirectMessagesHook?.Enable();
        OnNetworkChatHook?.Enable();
    }

    private byte IsAllowedToReceiveDirectMessagesDetour(nint a1, int checkType, byte a3, byte a4)
    {
        Log.Verbose("Just received a whisper.");

        byte returnValue = IsAllowedToReceiveDirectMessagesHook!.OriginalDisposeSafe(a1, checkType, a3, a4);

        if (!Configuration.Enabled)
        {
            return returnValue;
        }

        if (returnValue != 0)
        {
            return returnValue;
        }

        if (checkType != 2)
        {
            return returnValue;
        }

        manuallyOverwrote = true;

        return 1;
    }

    private void OnNetworkChatDetour(nint a1, MessagePacket* messagePacket)
    {
        try
        {
            Log.Verbose($"Just received a whisper from:" +
                $"{Environment.NewLine}[{messagePacket->SenderContentId}, {messagePacket->SenderAccountId}]" +
                $"{Environment.NewLine}[{messagePacket->MessageType}]" +
                $"{Environment.NewLine}[{messagePacket->GetSenderName()}@{messagePacket->SenderWorldId}]" +
                $"{Environment.NewLine}[{messagePacket->GetMessage()}]");
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

        // This means we forced the message to display on our client.
        // Let's tell the server we didn't see it though.
        if (manuallyOverwrote)
        {
            SendMessageFailed(messagePacket->SenderAccountId);
        }

        manuallyOverwrote = false;
    }

    private void SendMessageFailed(ulong accountId)
    {
        try
        {
            if (Framework.Instance()->NetworkModuleProxy == null)
            {
                Log.Debug("Sending message failed because NetworkModuleProxy is NULL.");

                return;
            }

            if (MessageBlockedDetour(Framework.Instance()->NetworkModuleProxy, accountId) != 1)
            {
                Log.Debug("Calling MessageBlockedDetour failed internally... well, nothign we can do about it now c:");
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Failure in SendMessageFailed.");
        }
    }

    private char MessageBlockedDetour(NetworkModuleProxy* networkModuleProxy, ulong accountId)
    {
        Log.Verbose("Handle message blocked.");

        return MessageBlockedHook!.OriginalDisposeSafe(networkModuleProxy, accountId);
    }

    public void Dispose()
    {
        IsAllowedToReceiveDirectMessagesHook?.Dispose();
        OnNetworkChatHook?.Dispose();
        MessageBlockedHook?.Dispose();
    }
}
