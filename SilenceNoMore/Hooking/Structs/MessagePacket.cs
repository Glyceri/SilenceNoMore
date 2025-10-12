using InteropGenerator.Runtime;
using System.Runtime.InteropServices;

namespace SilenceNoMore.Hooking.Structs;

[StructLayout(LayoutKind.Explicit)]
public struct MessagePacket
{
    [FieldOffset(0x00)] public ulong          SenderAccountId;
    [FieldOffset(0x08)] public ulong          SenderContentId;
    [FieldOffset(0x10)] public byte           SenderWorldId;
    [FieldOffset(0x13)] public CStringPointer SenderName;
    [FieldOffset(0x18)] public byte           MessageType;
    [FieldOffset(0x33)] public CStringPointer Message;
}
