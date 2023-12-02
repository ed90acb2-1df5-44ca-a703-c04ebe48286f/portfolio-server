using System.Threading.Tasks;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Errors;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Controllers;

public class RegistrationController : IController<RegistrationCommand>
{
    private readonly INetwork _network;
    private readonly Authentication _authentication;

    public RegistrationController(INetwork network, Authentication authentication)
    {
        _network = network;
        _authentication = authentication;
    }

    public async Task Handle(Connection connection, RegistrationCommand command)
    {
        var isRegistered = await _authentication.Register(command.Login, command.Password);
        var errorCode = isRegistered ? ErrorCode.Success : ErrorCode.RegistrationLoginExists;

        _network.Direct(connection, new RegistrationResponse { ErrorCode = errorCode }, DeliveryMethod.Reliable);
    }
}
