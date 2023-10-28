using System.Collections.Concurrent;

namespace Portfolio.Server.Net;

public class SessionStorage
{
    private readonly ConcurrentDictionary<Player, Session> _sessions = new();

    public void Create(Session session)
    {
        _sessions[session.Player] = session;
    }

    public bool Contains(Player player)
    {
        return _sessions.ContainsKey(player);
    }
}
