using Portfolio.Application.Security;

namespace Portfolio.Startup.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    public bool Verify(string text, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(text, hash);
    }
}
