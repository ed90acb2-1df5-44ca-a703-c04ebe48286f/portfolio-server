using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portfolio.Protocol.Packets;

namespace Portfolio.Server;

public class GameLoop : BackgroundService
{
    private readonly ILogger<GameLoop> _logger;
    private readonly ServerNetworkingService _server;

    public GameLoop(ILogger<GameLoop> logger, ServerNetworkingService server)
    {
        _logger = logger;
        _server = server;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting game loop.");

        _server.RegisterPacketHandler<LoginRequest>(packet =>
        {
            _logger.LogDebug($"Message Data: {packet.Login}:{packet.Password}");
            _server.Broadcast(new LoginResponse() {Token = Guid.NewGuid().ToString()});
        });

        _server.Start();

        while (cancellationToken.IsCancellationRequested == false)
        {
            await Task.Delay(1000, cancellationToken);

            _server.Update();
        }

        _server.Stop();
    }
}
