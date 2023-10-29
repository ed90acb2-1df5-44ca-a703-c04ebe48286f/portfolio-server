namespace Portfolio.Protocol
{
    public enum Opcode : uint
    {
        Undefined = 0,
        LoginCommand,
        LoginMessage,
        BroadcastMessage,
    }
}
