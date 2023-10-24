namespace Portfolio.Protocol
{
    public interface IPacketWriter
    {
        byte[] Data();

        void Reset();

        void WriteULong(ulong value);

        void WriteString(string value);
    }
}
