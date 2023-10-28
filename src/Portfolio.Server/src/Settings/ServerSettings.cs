namespace Portfolio.Server.Settings;

public class ServerSettings
{
    public bool UseNativeSockets { get; set; }
    public int MaxPeers { get; set; }
    public int Port { get; set; }
    public string Secret { get; set; } = string.Empty;
}
