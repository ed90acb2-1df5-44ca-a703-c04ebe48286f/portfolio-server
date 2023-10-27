namespace Portfolio.Server.Handlers
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler
    {
        Task Handle(int peerId, TCommand command);
    }
}
