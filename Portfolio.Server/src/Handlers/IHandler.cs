using Portfolio.Protocol;

namespace Portfolio.Server.Handlers;

public interface IHandler
{
}

public interface IHandler<in TCommand> : IHandler where TCommand : struct, ICommand
{
    Task Handle(int peerId, TCommand command);
}
