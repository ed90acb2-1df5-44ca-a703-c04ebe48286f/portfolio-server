using System.Threading.Tasks;
using Portfolio.Server.Controllers;

namespace Portfolio.Server.Net;

public interface IEndpointHandler
{
    Task Handle<TRequest, TController>(Connection connection, TRequest request)
        where TController : IController<TRequest>;
}
