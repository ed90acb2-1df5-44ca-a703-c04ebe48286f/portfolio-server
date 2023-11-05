using Portfolio.Server.Controllers;

namespace Portfolio.Server.Net;

public interface INetworking
{
    void Start();

    void Update();

    void Stop();

    void Send<TMessage>(Connection connection, TMessage packet, DeliveryMethod deliveryMethod = DeliveryMethod.Reliable);

    void Broadcast<TPacket>(TPacket packet, DeliveryMethod deliveryMethod);

    void BroadcastExcept<TPacket>(TPacket packet, DeliveryMethod deliveryMethod, Connection connection);

    void RegisterController<TPacket, TController>()
        where TController : IController<TPacket>
        where TPacket : new();
}
