using LiteNetLib.Utils;
using Portfolio.Protocol;

namespace Portfolio.Net
{
    public class LiteNetLibPacketReader : IPacketReader
    {
        public NetDataReader Reader = null!;

        public byte[] Data()
        {
            return Reader.RawData;
        }

        public ulong ReadULong()
        {
            return Reader.GetULong();
        }

        public string ReadString()
        {
            return Reader.GetString();
        }
    }
}
