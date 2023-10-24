namespace Portfolio.Protocol.Packets
{
    public interface IPacket
    {
        public void Serialize(IPacketWriter writer);

        public void Deserialize(IPacketReader reader);
    }
}
