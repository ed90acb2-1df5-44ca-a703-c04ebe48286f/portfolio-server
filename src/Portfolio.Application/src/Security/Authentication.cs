using System.Threading.Tasks;
using Portfolio.Application.Net;
using Portfolio.Application.Repositories;

namespace Portfolio.Application.Security;

public class Authentication
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SessionStorage _sessionStorage;

    public Authentication(IUserRepository userRepository, IPasswordHasher passwordHasher, SessionStorage sessionStorage)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionStorage = sessionStorage;
    }

    public bool IsAuthenticated(Connection connection)
    {
        return _sessionStorage.Contains(connection);
    }

    public async Task<bool> Authenticate(Connection connection, string login, string password)
    {
        var user = await _userRepository.FindByLoginAsync(login);

        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return false;
        }

        _sessionStorage.Create(new Session(connection, user));

        return true;
    }
}
