using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portfolio.Server.Repositories;

namespace Portfolio.Server.Services;

public class GameService : BackgroundService
{
    private readonly ILogger<GameService> _logger;
    private readonly NetworkingService _networking;
    private readonly IUserRepository _userRepository;

    public GameService(ILogger<GameService> logger, NetworkingService networking, IUserRepository userRepository)
    {
        _logger = logger;
        _networking = networking;
        _userRepository = userRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting game loop...");

        _networking.Start();

        while (cancellationToken.IsCancellationRequested == false)
        {
            await Task.Delay(1, cancellationToken);

            foreach (var user in await _userRepository.FindAll())
            {
                //_logger.LogDebug($"{user.Id}, {user.Login}, {user.PasswordHash}, {user.CreatedAt}, {user.UpdatedAt}");
            }

            _networking.Update();
        }

        _networking.Stop();
    }
}
