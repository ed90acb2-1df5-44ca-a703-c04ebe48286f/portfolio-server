namespace Portfolio.Server.Net;

public interface INetwork
{
    void Direct<TMessage>(Connection connection, TMessage message, DeliveryMethod deliveryMethod);

    void Fanout<TMessage>(TMessage message, DeliveryMethod deliveryMethod);

    void FanoutExcept<TMessage>(TMessage message, DeliveryMethod deliveryMethod, Connection connection);
}
