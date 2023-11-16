using System.Collections.Concurrent;
using Portfolio.Server.Models;
using Portfolio.Server.Net;

namespace Portfolio.Startup.Net;

public class InMemorySessionStorage : ISessionStorage
{
    private readonly ConcurrentDictionary<Connection, Session> _sessions = new();

    public void Create(Connection connection, User user)
    {
        _sessions[connection] = new Session(connection, user);
    }

    public bool Exists(Connection connection)
    {
        return _sessions.ContainsKey(connection);
    }
}
