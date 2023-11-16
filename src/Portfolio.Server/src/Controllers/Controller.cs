using System;
using System.Threading.Tasks;
using Portfolio.Server.Net;

namespace Portfolio.Server.Controllers;

public abstract class Controller<TRequest> : IController<TRequest>
{
    public async Task Handle(Connection connection, TRequest request)
    {
        try
        {
            await OnRequest(connection, request);
        }
        catch (Exception exception)
        {
            // ...
        }

    }

    protected abstract Task OnRequest(Connection connection, TRequest request);
}
