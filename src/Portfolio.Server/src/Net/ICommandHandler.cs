using System.Threading.Tasks;
using Portfolio.Server.Controllers;

namespace Portfolio.Server.Net;

public interface ICommandHandler
{
    Task Handle<TCommand, TController>(Connection connection, TCommand command)
        where TController : IController<TCommand>;
}
