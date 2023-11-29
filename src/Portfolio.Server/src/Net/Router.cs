using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portfolio.Protocol;
using Portfolio.Server.Controllers;

namespace Portfolio.Server.Net;

public class Router
{
    private readonly Dictionary<ulong, Func<Connection, byte[], Task>> _handlers = new();

    private readonly ILogger _logger;
    private readonly IEndpointHandler _endpointHandler;
    private readonly IPacketSerializer _packetSerializer;

    public Router(ILogger logger, IEndpointHandler endpointHandler, IPacketSerializer packetSerializer)
    {
        _logger = logger;
        _endpointHandler = endpointHandler;
        _packetSerializer = packetSerializer;
    }

    public Endpoint CreateEndpoint<TRequest, TController>()
        where TController : IController<TRequest> where TRequest : new()
    {
        var request = new TRequest();
        var endpoint = new Endpoint<TRequest, TController>(_logger, _endpointHandler);

        _handlers[Opcode.Get<TRequest>()] = async (connection, packet) =>
        {
            try
            {
                _packetSerializer.Hydrate(request, packet);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return;
            }

            await endpoint.Handle(connection, request);
        };

        return endpoint;
    }

    public async Task Route(Connection connection, ulong opcode, byte[] packet)
    {
        if (_handlers.TryGetValue(opcode, out var handler))
        {
            await handler.Invoke(connection, packet);
            return;
        }

        _logger.Error($"Missing handler for opcode: '{Opcode.Type(opcode)}' '{opcode}'");
    }
}
