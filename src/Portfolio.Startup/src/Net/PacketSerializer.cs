using Google.Protobuf;
using Portfolio.Server.Net;

namespace Portfolio.Startup.Net;

public class PacketSerializer : IPacketSerializer
{
    public void Hydrate<T>(T packet, byte[] data)
    {
        ((IMessage) packet!).MergeFrom(data);
    }
}
