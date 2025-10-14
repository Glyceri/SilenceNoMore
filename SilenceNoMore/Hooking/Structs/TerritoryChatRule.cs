using SilenceNoMore.Hooking.Enums;
using System.Runtime.InteropServices;

namespace SilenceNoMore.Hooking.Structs;

[StructLayout(LayoutKind.Explicit)]
internal struct TerritoryChatRule
{
    [FieldOffset(0x00)] public ChatBlockStateEnum Public;
    [FieldOffset(0x01)] public ChatBlockStateEnum Shout; 
    [FieldOffset(0x02)] public ChatBlockStateEnum DutyTell;
    [FieldOffset(0x03)] public ChatBlockStateEnum PublicTell;
    [FieldOffset(0x04)] public ChatBlockStateEnum Party;
    [FieldOffset(0x05)] public ChatBlockStateEnum Global;
    [FieldOffset(0x06)] public ChatBlockStateEnum Pvp;
    [FieldOffset(0x07)] public bool               UnkBool;
}
