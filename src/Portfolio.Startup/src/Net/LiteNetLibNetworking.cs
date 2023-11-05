using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Server.Controllers;
using Portfolio.Server.Net;
using Portfolio.Protocol;
using Portfolio.Protocol.Authentication;
using Portfolio.Server.Security;
using Portfolio.Startup.Settings;
using DeliveryMethod = Portfolio.Server.Net.DeliveryMethod;

namespace Portfolio.Startup.Net
{
    public class LiteNetLibNetworking : INetworking, INetEventListener
    {
        private readonly ConcurrentDictionary<Connection, NetPeer> _peers = new();
        private readonly Dictionary<ulong, Func<NetPeer, NetDataReader, Task>> _handlers = new();
        private readonly BufferWriter _buffer = new();
        private readonly NetManager _manager;

        private readonly ILogger<LiteNetLibNetworking> _logger;
        private readonly IOptions<NetworkingSettings> _options;
        private readonly IServiceProvider _serviceProvider;

        public LiteNetLibNetworking(ILogger<LiteNetLibNetworking> logger, IOptions<NetworkingSettings> options, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _options = options;
            _serviceProvider = serviceProvider;

            _manager = new NetManager(this);
            _manager.UseNativeSockets = options.Value.UseNativeSockets;
            _manager.AutoRecycle = true;
        }

        public void RegisterController<TPacket, TController>() where TController : IController<TPacket> where TPacket : new()
        {
            var packet = new TPacket();

            // TODO: Middlewares
            var authentication = _serviceProvider.GetRequiredService<Authentication>();
            var authenticationRequired = !(packet is LoginRequest || packet is RegistrationRequest);

            _handlers[Opcodes.Get<TPacket>()] = async (peer, reader) =>
            {
                var connection = (Connection) peer.Tag;

                if (authenticationRequired && authentication.IsAuthenticated(connection))
                {
                    _logger.LogWarning($"Unauthenticated call to: {nameof(TController)}");
                    return;
                }

                try
                {
                    Hydrate(packet, reader.GetRemainingBytes());

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    await scope.ServiceProvider.GetRequiredService<TController>().Handle(connection, packet);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);
                }
            };
        }

        public void Start()
        {
            _logger.LogInformation("Starting server...");

            _manager.Start(_options.Value.Port);

            _logger.LogInformation($"Server started at port: {_manager.LocalPort}");
        }

        public void Update()
        {
            _manager.PollEvents();
        }

        public void Stop()
        {
            _manager.Stop();
        }

        public void Send<TPacket>(Connection connection, TPacket packet, DeliveryMethod deliveryMethod)
        {
            lock (_buffer)
            {
                Serialize(packet, _buffer);
                _peers[connection].Send(_buffer.AsSpan(), LiteNetLib.DeliveryMethod.ReliableOrdered);
            }
        }

        public void Broadcast<TPacket>(TPacket packet, DeliveryMethod deliveryMethod)
        {
            lock (_buffer)
            {
                Serialize(packet, _buffer);
                _manager.SendToAll(_buffer.Buffer, 0, _buffer.Position, LiteNetLib.DeliveryMethod.ReliableOrdered);
            }
        }

        public void BroadcastExcept<TPacket>(TPacket packet, DeliveryMethod deliveryMethod, Connection connection)
        {
            lock (_buffer)
            {
                Serialize(packet, _buffer);
                _manager.SendToAll(_buffer.Buffer, 0, _buffer.Position, LiteNetLib.DeliveryMethod.ReliableOrdered, _peers[connection]);
            }
        }

        public void Disconnect(Connection connection)
        {
            if (_peers.TryGetValue(connection, out var peer))
            {
                peer.Disconnect();
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            _logger.LogDebug("OnConnectionRequest");

            if (_manager.ConnectedPeersCount <= _options.Value.MaxPeers)
            {
                request.AcceptIfKey(_options.Value.Secret);
            }
            else
            {
                request.Reject();
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _logger.LogDebug("OnPeerConnected");

            var player = new Connection(peer.Id);
            peer.Tag = player;

            _peers.TryAdd(player, peer);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, LiteNetLib.DeliveryMethod deliveryMethod)
        {
            //_logger.LogDebug("OnNetworkReceive");

            var opcode = ReadOpcode(reader);

            if (_handlers.TryGetValue(opcode, out var handler))
            {
                handler.Invoke(peer, reader);
            }
            else
            {
                _logger.LogWarning($"Handler not found. Opcode: {opcode.ToString()}");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _logger.LogDebug("OnNetworkLatencyUpdate");

            var player = (Connection) peer.Tag;
            player.Ping = latency;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.LogDebug("OnPeerDisconnected");

            var player = (Connection) peer.Tag;

            _peers.TryRemove(player, out _);

            peer.Tag = null;
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            _logger.LogDebug("OnNetworkError");
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            _logger.LogDebug("OnNetworkReceiveUnconnected");
        }

        private static void Serialize<TPacket>(TPacket packet, BufferWriter buffer)
        {
            buffer.Reset();
            buffer.Write(Opcodes.Get<TPacket>());

            ((IMessage) packet!).WriteTo(buffer);
        }

        private static void Hydrate<TPacket>(TPacket packet, byte[] data)
        {
            ((IMessage) packet!).MergeFrom(data);
        }

        private static ulong ReadOpcode(NetDataReader reader)
        {
            return reader.GetULong();
        }
    }
}
