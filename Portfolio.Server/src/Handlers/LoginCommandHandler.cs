using Microsoft.Extensions.Logging;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;
using Portfolio.Server.Services;

namespace Portfolio.Server.Handlers;

public class LoginCommandHandler : IHandler<LoginCommand>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly AuthenticationService _authenticationService;
    private readonly SessionService _sessionService;
    private readonly NetworkingService _networkingService;

    public LoginCommandHandler(
        ILogger<LoginCommandHandler> logger,
        AuthenticationService authenticationService,
        SessionService sessionService,
        NetworkingService networkingService)
    {
        _logger = logger;
        _authenticationService = authenticationService;
        _sessionService = sessionService;
        _networkingService = networkingService;
    }

    public async Task Handle(int peerId, LoginCommand command)
    {
        var user = await _authenticationService.Authenticate(command.Login, command.Password);

        _logger.LogDebug("LoginCommandHandler: {Thread}", Environment.CurrentManagedThreadId.ToString());
        _networkingService.Send(peerId, new LoginMessage() {Token = $"{command.Login}:{command.Password}"});

        if (user == null)
        {
            _networkingService.Kick(peerId);
            return;
        }

        _sessionService.Register(peerId, user);
    }
}
