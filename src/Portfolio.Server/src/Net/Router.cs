using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portfolio.Common;
using Portfolio.Server.Controllers;

namespace Portfolio.Server.Net;

public class Router
{
    private readonly Dictionary<ulong, Func<Connection, byte[], Task>> _handlers = new();

    private readonly ILogger _logger;
    private readonly ICommandHandler _commandHandler;
    private readonly IPacketSerializer _packetSerializer;

    public Router(ILogger logger, ICommandHandler commandHandler, IPacketSerializer packetSerializer)
    {
        _logger = logger;
        _commandHandler = commandHandler;
        _packetSerializer = packetSerializer;
    }

    public Endpoint CreateEndpoint<TCommand, TController>()
        where TController : IController<TCommand> where TCommand : notnull, new()
    {
        var command = new TCommand();
        var endpoint = new Endpoint<TCommand, TController>(_logger, _commandHandler);

        _handlers[TypeHash.Hash<TCommand>()] = async (connection, packet) =>
        {
            try
            {
                _packetSerializer.Hydrate(command, packet);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return;
            }

            await endpoint.Handle(connection, command);
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

        _logger.Error($"Missing handler for opcode: '{TypeHash.Type(opcode)}' '{opcode}'");
    }
}
