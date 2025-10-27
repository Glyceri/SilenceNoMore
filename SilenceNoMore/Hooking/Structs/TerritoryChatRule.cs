using SilenceNoMore.Hooking.Enums;
using System;
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

    public override string ToString()
        => $"[Territory Chat Rule]{Environment.NewLine}" +
           $"[Public]     [{Public}]{Environment.NewLine}" +
           $"[Shout]      [{Shout}]{Environment.NewLine}" +
           $"[DutyTell]   [{DutyTell}]{Environment.NewLine}" +
           $"[PublicTell] [{PublicTell}]{Environment.NewLine}" +
           $"[Party]      [{Party}]{Environment.NewLine}" +
           $"[Global]     [{Global}]{Environment.NewLine}" +
           $"[Pvp]        [{Pvp}]{Environment.NewLine}";
}
