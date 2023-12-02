namespace Portfolio.Gameplay;

public readonly struct Message<T>
{
    public readonly int PlayerId;
    public readonly MessageReceiver Receiver;
    public readonly T Payload;

    public Message(int playerId, MessageReceiver receiver, T payload)
    {
        PlayerId = playerId;
        Receiver = receiver;
        Payload = payload;
    }
}
