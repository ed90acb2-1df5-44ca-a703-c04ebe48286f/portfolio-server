using Portfolio.Server.Net;

namespace Portfolio.Server.CommandHandlers
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler
    {
        Task Handle(Player player, TCommand command);
    }
}
