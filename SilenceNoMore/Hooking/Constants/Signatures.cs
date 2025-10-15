namespace SilenceNoMore.Hooking.Constants;

internal static class Signatures
{
    public const string ExecuteTellCommandSignature                 = "40 55 53 56 57 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? B8";
    public const string GetTerritoryIntendedUseSignature            = "E8 ?? ?? ?? ?? 8B BE ?? ?? ?? ?? 4C 8B E0";
    public const string GetTerritoryChatRuleSignature               = "E8 ?? ?? ?? ?? 48 85 C0 74 1A 80 78 04 02";
    public const string SendPublicTellSignature                     = "E8 ?? ?? ?? ?? 48 8B 16 48 8B CE FF 52 58 48 8B 16";

    public const string IsAllowedToReceiveDirectMessagesSignature   = "48 89 5C 24 ?? 57 48 83 EC 20 48 63 FA 41 0F B6 D8";
    public const string OnNetworkChatSignature                      = "41 54 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 4C 8B E2";
    public const string MessageBlockedSignature                     = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 89 B4 24 ?? ?? ?? ?? 33 F6 41 80 7C 24";
}
