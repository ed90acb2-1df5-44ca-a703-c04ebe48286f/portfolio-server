using Portfolio.Server.Entities;
using Portfolio.Server.Repositories;

namespace Portfolio.Server.Services;

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Register(string login, string password)
    {
        // ...

        return null;
    }

    public async Task<User?> Authenticate(string login, string password)
    {
        var user = await _userRepository.GetByLoginAsync(login);

        if (user is null)
        {
            return null;
        }

        return BCrypt.Net.BCrypt.Verify(user.PasswordHash, password) ? user : null;
    }
}
