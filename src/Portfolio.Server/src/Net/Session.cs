using Portfolio.Server.Entities;

namespace Portfolio.Server.Net;

public class Session
{
    public readonly Player Player;
    public readonly User User;

    public Session(Player player, User user)
    {
        Player = player;
        User = user;
    }
}
