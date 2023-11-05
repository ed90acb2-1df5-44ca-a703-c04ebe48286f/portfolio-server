using System;
using System.Threading.Tasks;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Errors;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Controllers;

public class LoginController : IController<LoginRequest>
{
    private readonly ILogger _logger;
    private readonly Authentication _authentication;
    private readonly INetworking _networking;

    public LoginController(ILogger logger, Authentication authentication, INetworking networking)
    {
        _logger = logger;
        _authentication = authentication;
        _networking = networking;
    }

    public async Task Handle(Connection connection, LoginRequest request)
    {
        var isAuthenticated = await _authentication.Authenticate(connection, request.Login, request.Password);

        _networking.Send(connection, new LoginResponse
        {
            ErrorCode = isAuthenticated ? ErrorCode.Success : ErrorCode.AuthenticationInvalidCredentials
        });

        _logger.Debug($"LoginCommandHandler: {Environment.CurrentManagedThreadId.ToString()}");
    }
}
