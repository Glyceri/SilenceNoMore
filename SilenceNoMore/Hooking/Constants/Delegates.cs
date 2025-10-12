using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.Shell;
using SilenceNoMore.Hooking.Structs;

namespace SilenceNoMore.Hooking.Constants;

internal static unsafe class Delegates
{
    public delegate int  ExecuteTellCommandDelegate                 (ShellCommands* shellCommands, Utf8String* tell, UIModule* uiModule);
    public delegate nint GetTerritoryIntendedUseDelegate            (uint rowIdOrIndex);

    public delegate byte IsAllowedToReceiveDirectMessagesDelegate   (RaptureShellModule* raptureShell, int checkType, byte checkForInnStatus, byte isLocked);
    public delegate void OnNetworkChatDelegate                      (nint unknownBase, MessagePacket* messagePacket);
    public delegate char MessageBlockedDelegate                     (NetworkModuleProxy* networkModuleProxy, ulong contentId);
}
