namespace Portfolio.Server.Net;

public interface INetwork
{
    void Send<TMessage>(Connection connection, TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.Reliable);

    void Broadcast<TMessage>(TMessage message, DeliveryMethod deliveryMethod);

    void BroadcastExcept<TMessage>(TMessage message, DeliveryMethod deliveryMethod, Connection connection);
}
