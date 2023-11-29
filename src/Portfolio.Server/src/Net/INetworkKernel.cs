namespace Portfolio.Server.Net;

public interface INetworkKernel : INetwork
{
    void SetRouter(Router router);

    void Start();

    void Update();

    void Stop();
}
