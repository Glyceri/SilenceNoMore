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

    public TellReceivedHook(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration)
        : base(hooker, log, configuration) { }

    public override void Init()
    {
        IsAllowedToReceiveDirectMessagesHook?.Enable();
        OnNetworkChatHook?.Enable();
    }

    private byte IsAllowedToReceiveDirectMessagesDetour(RaptureShellModule* a1, int checkType, byte a3, byte a4)
    {
        Log.Verbose("Je hebt zojuist een fluisterbericht ontvangen.");

        byte returnValue = IsAllowedToReceiveDirectMessagesHook!.OriginalDisposeSafe(a1, checkType, a3, a4);

        if (!isValidCheck)
        {
            return returnValue;
        }

        if (!Configuration.IsEnabled)
        {
            Log.Verbose("'Configuration.IsEnabled' heeft de waarde 'False'.");

            return returnValue;
        }

        if (returnValue != 0)
        {
            Log.Verbose("'returnValue' is niet gelijk aan 0.");

            return returnValue;
        }

        if (checkType != 2)
        {
            Log.Verbose("'checkType' is niet gelijk aan 2.");

            return returnValue;
        }

        Log.Verbose("Het fluisterbericht is overschreven.");

        manuallyOverwrote = true;

        return 1;
    }

    private void OnNetworkChatDetour(nint a1, MessagePacket* messagePacket)
    {
        isValidCheck = true;

        try
        {
            Log.Verbose($"Je hebt zojuist een fluisterbericht ontvangen van:" +
                $"{Environment.NewLine}[{messagePacket->SenderContentId}]" +
                $"{Environment.NewLine}[{messagePacket->MessageType}]");
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout in de 'OnNetworkChatDetour'.");
        }

        try
        {
            OnNetworkChatHook?.OriginalDisposeSafe(a1, messagePacket);
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout in de 'OnNetworkChatHook'.");
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
            Log.Verbose($"'isValidCheck' heeft gefaald.");

            return;
        }

        try
        {
            if (Framework.Instance()->NetworkModuleProxy == null)
            {
                Log.Debug("Het terugsturen van de waarschuwing is mislukt omdat NetworkModuleProxy NULL is.");

                return;
            }

            if (MessageBlockedDetour(Framework.Instance()->NetworkModuleProxy, contentId) != 1)
            {
                Log.Debug("Het aanroepen van 'MessageBlockedDetour' heeft intern een fout opgeleverd.");
            }

            Log.Verbose($"Heeft zojuist de 'busy' waarschuwing verstuurd naar de verzender zijn 'Client'.");
        }
        catch (Exception e)
        {
            Log.Error(e, "Fout in 'SendMessageFailed'.");
        }
    }

    private char MessageBlockedDetour(NetworkModuleProxy* networkModuleProxy, ulong contentId)
    {
        Log.Verbose("Behandel 'Message Blocked' voor de gebruiker met het 'ContentId': " + contentId);

        return MessageBlockedHook!.OriginalDisposeSafe(networkModuleProxy, contentId);
    }

    public override void Dispose()
    {
        IsAllowedToReceiveDirectMessagesHook?.Dispose();
        OnNetworkChatHook?.Dispose();
        MessageBlockedHook?.Dispose();
    }
}
