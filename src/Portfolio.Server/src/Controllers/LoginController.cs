using System.Threading.Tasks;
using Portfolio.Gameplay;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Errors;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Controllers;

public class LoginController : IController<LoginCommand>
{
    private readonly Game _game;
    private readonly ILogger _logger;
    private readonly Authentication _authentication;
    private readonly INetwork _network;

    public LoginController(Game game, ILogger logger, Authentication authentication, INetwork network)
    {
        _game = game;
        _logger = logger;
        _authentication = authentication;
        _network = network;
    }

    public async Task Handle(Connection connection, LoginCommand command)
    {
        var isAuthenticated = await _authentication.Authenticate(connection, command.Login, command.Password);

        if (!isAuthenticated)
        {
            _network.Direct(connection, new LoginResponse { ErrorCode = ErrorCode.AuthenticationInvalidCredentials }, DeliveryMethod.Reliable);
            return;
        }

        _network.Direct(connection, new LoginResponse { ErrorCode = ErrorCode.Success }, DeliveryMethod.Reliable);

        _game.SpawnPlayer(connection.Id);
    }
}
