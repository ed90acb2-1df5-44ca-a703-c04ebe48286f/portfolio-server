using System.Threading.Tasks;
using Portfolio.Server.Net;

namespace Portfolio.Server.Controllers
{
    public interface IController<in TCommand>
    {
        Task Handle(Connection connection, TCommand command);
    }
}
