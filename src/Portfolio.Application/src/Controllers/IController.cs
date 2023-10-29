using System.Threading.Tasks;
using Portfolio.Application.Net;

namespace Portfolio.Application.Controllers
{
    public interface IController<in TCommand>
    {
        Task Handle(Connection connection, TCommand command);
    }
}
