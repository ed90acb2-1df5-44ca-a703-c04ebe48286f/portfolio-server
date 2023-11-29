using System;
using System.Threading.Tasks;
using Portfolio.Gameplay;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Errors;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Controllers;

public class LoginController : IController<LoginRequest>
{
    private readonly Game _game;
    private readonly ILogger _logger;
    private readonly Authentication _authentication;
    private readonly INetworkKernel _network;

    public LoginController(Game game, ILogger logger, Authentication authentication, INetworkKernel network)
    {
        _game = game;
        _logger = logger;
        _authentication = authentication;
        _network = network;
    }

    public async Task Handle(Connection connection, LoginRequest request)
    {
        _logger.Debug($"{GetType().Name}: {Environment.CurrentManagedThreadId.ToString()}");

        var isAuthenticated = await _authentication.Authenticate(connection, request.Login, request.Password);

        if (!isAuthenticated)
        {
            _network.Send(connection, new LoginResponse { ErrorCode = ErrorCode.AuthenticationInvalidCredentials });
            return;
        }

        _network.Send(connection, new LoginResponse { ErrorCode = ErrorCode.Success });

        _game.SpawnPlayer(connection.Id);
    }
}
