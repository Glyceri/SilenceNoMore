using System.Runtime.InteropServices;
using System.Text;

namespace SilenceNoMore;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public unsafe struct MessagePacket
{
    [FieldOffset(0x00)] public ulong SenderContentId;
    [FieldOffset(0x08)] public ulong SenderAccountId;
    [FieldOffset(0x10)] public byte SenderWorldId;
    [FieldOffset(0x13)] public fixed byte SenderName[32];
    [FieldOffset(0x18)] public byte MessageType;
    [FieldOffset(0x33)] public fixed byte Message[512];

    public string GetSenderName()
    {
        fixed (byte* p = SenderName)
        {
            return ReadUtf8(p, 32);
        }
    }

    public string GetMessage()
    {
        fixed (byte* p = Message)
        {
            return ReadUtf8(p, 512);
        }
    }

    private static string ReadUtf8(byte* ptr, int maxLen)
    {
        int len = 0;

        // Null terminator check c:
        while (len < maxLen && ptr[len] != 0)
        {
            len++;
        }

        return Encoding.UTF8.GetString(ptr, len);
    }
}
