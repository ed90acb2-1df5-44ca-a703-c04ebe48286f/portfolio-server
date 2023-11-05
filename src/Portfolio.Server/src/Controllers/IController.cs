using System.Threading.Tasks;
using Portfolio.Server.Net;

namespace Portfolio.Server.Controllers
{
    public interface IController<in TRequest>
    {
        Task Handle(Connection connection, TRequest request);
    }
}
