using System;
using System.Threading.Tasks;
using Portfolio.Application.Net;
using Portfolio.Application.Security;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;

namespace Portfolio.Application.Controllers;

public class LoginController : IController<LoginCommand>
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

    public async Task Handle(Connection connection, LoginCommand command)
    {
        var success = await _authentication.Authenticate(connection, command.Login, command.Password);

        _logger.Debug($"LoginCommandHandler: {Environment.CurrentManagedThreadId.ToString()}");
        _networking.Send(connection, new LoginMessage() {Token = $"{command.Login}:{command.Password}"});
    }
}
