using System.Threading.Tasks;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Errors;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Controllers;

public class RegistrationController : IController<RegistrationRequest>
{
    private readonly INetworkKernel _networkKernel;
    private readonly Authentication _authentication;

    public RegistrationController(INetworkKernel networkKernel, Authentication authentication)
    {
        _networkKernel = networkKernel;
        _authentication = authentication;
    }

    public async Task Handle(Connection connection, RegistrationRequest request)
    {
        var isRegistered = await _authentication.Register(request.Login, request.Password);

        _networkKernel.Send(connection, new RegistrationResponse
        {
            ErrorCode = isRegistered ? ErrorCode.Success : ErrorCode.RegistrationLoginExists
        });
    }
}
