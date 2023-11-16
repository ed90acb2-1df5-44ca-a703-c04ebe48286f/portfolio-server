namespace Portfolio.Server.Net;

public interface IPacketSerializer
{
    void Hydrate<T>(T packet, byte[] data);
}
