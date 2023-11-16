using Portfolio.Server.Models;

namespace Portfolio.Server.Net;

public interface ISessionStorage
{
    void Create(Connection connection, User user);

    bool Exists(Connection connection);
}
