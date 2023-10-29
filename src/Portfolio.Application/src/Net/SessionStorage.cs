using System.Collections.Concurrent;

namespace Portfolio.Application.Net;

public class SessionStorage
{
    private readonly ConcurrentDictionary<Connection, Session> _sessions = new();

    public void Create(Session session)
    {
        _sessions[session.Connection] = session;
    }

    public bool Contains(Connection connection)
    {
        return _sessions.ContainsKey(connection);
    }
}
