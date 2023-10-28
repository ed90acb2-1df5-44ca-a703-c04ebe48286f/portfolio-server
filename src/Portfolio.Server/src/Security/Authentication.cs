using Portfolio.Server.Net;
using Portfolio.Server.Repositories;

namespace Portfolio.Server.Security;

public class Authentication
{
    private readonly IUserRepository _userRepository;
    private readonly SessionStorage _sessionStorage;

    public Authentication(IUserRepository userRepository, SessionStorage sessionStorage)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
    }

    public bool IsAuthenticated(Player player)
    {
        return _sessionStorage.Contains(player);
    }

    public async Task<bool> Authenticate(Player player, string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login);

        if (user is null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, password))
        {
            return false;
        }

        _sessionStorage.Create(new Session(player, user));

        return true;
    }
}
