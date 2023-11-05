using Portfolio.Server.Security;

namespace Portfolio.Startup.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
