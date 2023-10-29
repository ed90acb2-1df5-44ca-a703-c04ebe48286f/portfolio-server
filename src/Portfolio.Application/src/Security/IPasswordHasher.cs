namespace Portfolio.Application.Security;

public interface IPasswordHasher
{
    bool Verify(string text, string hash);
}
