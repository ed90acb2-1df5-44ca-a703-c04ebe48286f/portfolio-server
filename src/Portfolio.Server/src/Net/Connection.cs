namespace Portfolio.Server.Net;

public class Connection
{
    public readonly int Id;
    public int Ping;

    public Connection(int id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Connection other && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
