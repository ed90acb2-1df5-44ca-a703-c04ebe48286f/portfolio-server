using Portfolio.Server.Models;

namespace Portfolio.Server.Net;

public class Session
{
    public readonly Connection Connection;
    public readonly User User;

    public Session(Connection connection, User user)
    {
        Connection = connection;
        User = user;
    }
}
