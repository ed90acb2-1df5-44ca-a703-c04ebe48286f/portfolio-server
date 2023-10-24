namespace Portfolio.Protocol
{
    public interface IPacketReader
    {
        byte[] Data();

        ulong ReadULong();

        string ReadString();
    }
}
