using System.Collections.Concurrent;
using Portfolio.Server.Entities;

namespace Portfolio.Server.Services;

public class SessionService
{
    private readonly ConcurrentDictionary<int, User> _sessions = new();

    public SessionService()
    {
    }

    public void Register(int peerId, User user)
    {
        _sessions[peerId] = user;
    }

    public void Deregister(int peerId)
    {
        _sessions.Remove(peerId, out _);
    }
}
