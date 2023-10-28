using Microsoft.Extensions.Logging;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.CommandHandlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly Authentication _authentication;
    private readonly NetworkDispatcher _networkDispatcher;

    public LoginCommandHandler(ILogger<LoginCommandHandler> logger, Authentication authentication, NetworkDispatcher networkDispatcher)
    {
        _logger = logger;
        _authentication = authentication;
        _networkDispatcher = networkDispatcher;
    }

    public async Task Handle(Player player, LoginCommand command)
    {
        var success = await _authentication.Authenticate(player, command.Login, command.Password);

        _logger.LogDebug("LoginCommandHandler: {Thread}", Environment.CurrentManagedThreadId.ToString());
        _networkDispatcher.Send(player, new LoginMessage() {Token = $"{command.Login}:{command.Password}"});
    }
}
