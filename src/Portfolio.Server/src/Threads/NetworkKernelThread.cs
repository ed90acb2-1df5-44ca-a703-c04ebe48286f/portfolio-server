using System;
using System.Threading;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Commands;
using Portfolio.Server.Controllers;
using Portfolio.Server.Filters;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Threads;

public class NetworkKernelThread
{
    private readonly ILogger _logger;
    private readonly INetworkKernel _networkKernel;

    public NetworkKernelThread(INetworkKernel networkKernel, Router router, Authentication authentication, ILogger logger)
    {
        _logger = logger;
        _networkKernel = networkKernel;

        router.CreateEndpoint<LoginCommand, LoginController>();
        router.CreateEndpoint<RegistrationCommand, RegistrationController>();

        new EndpointGroup()
            .Add(router.CreateEndpoint<InputCommand, InputController>())
            .Filter(new AuthenticationConnectionFilter(authentication));

        _networkKernel.SetRouter(router);
    }

    public void Start()
    {
        var thread = new Thread(ThreadStart);
        thread.Name = nameof(NetworkKernelThread);

        _logger.Information($"Starting {thread.Name}");

        thread.Start();
    }

    private void ThreadStart()
    {
        try
        {
            _networkKernel.Start();

            while (true)
            {
                _networkKernel.Update();
                Thread.Sleep(1000 / 60);
            }
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
