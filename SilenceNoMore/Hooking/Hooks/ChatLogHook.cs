using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using SilenceNoMore.Hooking.Enums;
using SilenceNoMore.TellHandling;

namespace SilenceNoMore.Hooking.Hooks;

internal unsafe class ChatLogHook : HookableElement
{
    private readonly IAddonLifecycle AddonLifecycle;
    private readonly TellHandler     TellHandler;

    private readonly Hook<AgentChatLog.Delegates.ChangeChannelName>? ChangeChannelNameHook = null!;

    private bool         chatIsTell       = false;

    public ChatLogHook(IGameInteropProvider hooker, IPluginLog log, IConfiguration configuration, IAddonLifecycle addonLifecycle, TellHandler tellHandler)
        : base(hooker, log, configuration) 
    {
        AddonLifecycle = addonLifecycle;
        TellHandler    = tellHandler;

        ChangeChannelNameHook = Hooker.HookFromAddress<AgentChatLog.Delegates.ChangeChannelName>(AgentChatLog.MemberFunctionPointers.ChangeChannelName, ChangeChannelNameDetour);
    }

    public override void Init()
    {
        ChangeChannelNameHook?.Enable();

        AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "ChatLog", LifecycleChatlogPostRequestedUpdateDetour);
    }

    private void LifecycleChatlogPostRequestedUpdateDetour(AddonEvent type, AddonArgs args)
        => ChatLogPostRequestedUpdateDetour((AtkUnitBase*)args.Addon.Address);

    private void ChatLogPostRequestedUpdateDetour(AtkUnitBase* atkUnitBase)
    {
        Log.Verbose("Heeft zojuist ChatLogPostRequestedUpdateDetour aangeroepen");

        if (atkUnitBase == null)
        {
            return;
        }

        if (!Configuration.ShouldAddToChatLabel)
        {
            return;
        }

        if (TellHandler.IsRestricted)
        {
            return;
        }

        // Chat type label
        AtkTextNode* labelTextNode = atkUnitBase->GetTextNodeById(4);

        if (labelTextNode == null)
        {
            return;
        }

        Log.Verbose($"De huidige labelTextNode heeft als text: {labelTextNode->NodeText.ToString()}");

        if (chatIsTell)
        {
            // Dit is ABSURD maar lost voor nu wel het probleem op ERM
            if (labelTextNode->NodeText.ToString().EndsWith(')'))
            {
                return;
            }

            labelTextNode->NodeText.Append(Utf8String.FromString($" ({TellHandler.TellStateName})"));

            Log.Verbose($"De huidige labelTextNode heeft als text: {labelTextNode->NodeText.ToString()}");
        }
    }

    private CStringPointer ChangeChannelNameDetour(AgentChatLog* thisPtr)
    {
        chatIsTell = false;

        Log.Verbose("Heeft de naam van het huidige 'chat' kannaal aangepast.");

        CStringPointer returner = ChangeChannelNameHook!.OriginalDisposeSafe(thisPtr);

        if (thisPtr == null)
        {
            return returner;
        }

        InputChannel chatType = (InputChannel)RaptureShellModule.Instance()->ChatType;

        Log.Verbose($"De gekozen chatType is: {chatType}");

        if (chatType == InputChannel.Unused1 || chatType == InputChannel.Unused2)
        {
            chatType = InputChannel.Tell;
        }

        if (chatType != InputChannel.Tell)
        {
            return returner;
        }


        string playerName = SeString.Parse(thisPtr->TellPlayerName).TextValue;
        ushort worldId    = thisPtr->TellWorldId;

        Log.Verbose($"De fluister naam is: {playerName}@{worldId}");

        chatIsTell = true;

        return returner;
    }

    public override void Dispose()
    {
        AddonLifecycle.UnregisterListener(LifecycleChatlogPostRequestedUpdateDetour);

        ChangeChannelNameHook?.Dispose();
    }
}
