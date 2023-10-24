using System;

namespace Portfolio.Protocol.Packets
{
    public class LoginResponse : IPacket
    {
        public string Token = String.Empty;

        public void Serialize(IPacketWriter writer)
        {
            writer.WriteString(Token);
        }

        public void Deserialize(IPacketReader reader)
        {
            Token = reader.ReadString();
        }
    }
}
