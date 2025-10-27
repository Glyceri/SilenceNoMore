using SilenceNoMore.Hooking.Enums;
using System.Runtime.InteropServices;

namespace SilenceNoMore.Hooking.Structs;

[StructLayout(LayoutKind.Explicit)]
internal struct TerritoryIntendedUse
{
    [FieldOffset(0x03)] public TerritoryChatRuleEnum TerritoryChatRule;
}
