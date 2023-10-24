using LiteNetLib.Utils;
using Portfolio.Protocol;

namespace Portfolio.Net
{
    public class LiteNetLibPacketWriter : IPacketWriter
    {
        public readonly NetDataWriter Writer;

        public LiteNetLibPacketWriter(NetDataWriter writer)
        {
            Writer = writer;
        }

        public byte[] Data()
        {
            return Writer.Data;
        }

        public void Reset()
        {
            Writer.Reset();
        }

        public void WriteULong(ulong value)
        {
            Writer.Put(value);
        }

        public void WriteString(string value)
        {
            Writer.Put(value);
        }
    }
}
