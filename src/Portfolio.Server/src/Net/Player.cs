namespace Portfolio.Server.Net;

public class Player
{
    public readonly int Id;
    public int Ping;

    public Player(int id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player other && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
