using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Server.Net;
using Portfolio.Protocol;
using Portfolio.Startup.Settings;
using DeliveryMethod = Portfolio.Server.Net.DeliveryMethod;

namespace Portfolio.Startup.Net
{
    public class LiteNetLibNetworkKernel : INetworkKernel, INetEventListener
    {
        private readonly Dictionary<Connection, NetPeer> _peers = new();
        private readonly BufferWriter _buffer = new();
        private readonly NetManager _manager;

        private readonly ILogger<LiteNetLibNetworkKernel> _logger;
        private readonly IOptions<NetworkingSettings> _options;

        private Router _router = null!;

        public LiteNetLibNetworkKernel(ILogger<LiteNetLibNetworkKernel> logger, IOptions<NetworkingSettings> options)
        {
            _logger = logger;
            _options = options;

            _manager = new NetManager(this);
            _manager.UseNativeSockets = options.Value.UseNativeSockets;
            _manager.AutoRecycle = true;
        }

        public void SetRouter(Router router)
        {
            _router = router;
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

        public void Send<TPacket>(Connection connection, TPacket message, DeliveryMethod deliveryMethod)
        {
            lock (_buffer)
            {
                Serialize(message, _buffer);
                _peers[connection].Send(_buffer.AsSpan(), LiteNetLib.DeliveryMethod.ReliableOrdered);
            }
        }

        public void Broadcast<TPacket>(TPacket message, DeliveryMethod deliveryMethod)
        {
            lock (_buffer)
            {
                Serialize(message, _buffer);
                _manager.SendToAll(_buffer.Buffer, 0, _buffer.Position, LiteNetLib.DeliveryMethod.ReliableOrdered);
            }
        }

        public void BroadcastExcept<TPacket>(TPacket message, DeliveryMethod deliveryMethod, Connection connection)
        {
            lock (_buffer)
            {
                Serialize(message, _buffer);
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

            var connection = new Connection(peer.Id);
            peer.Tag = connection;

            _peers.Add(connection, peer);
        }

        public async void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, LiteNetLib.DeliveryMethod deliveryMethod)
        {
            //_logger.LogDebug("OnNetworkReceive");

            var opcode = ReadOpcode(reader);
            await _router.Route((Connection) peer.Tag, opcode, reader.GetRemainingBytes());
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //_logger.LogDebug("OnNetworkLatencyUpdate");

            var player = (Connection) peer.Tag;
            player.Ping = latency;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.LogDebug("OnPeerDisconnected");

            var player = (Connection) peer.Tag;

            _peers.Remove(player);

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

        private static void Serialize<T>(T packet, BufferWriter buffer)
        {
            buffer.Reset();
            buffer.Write(Opcode.Get<T>());

            ((IMessage) packet!).WriteTo(buffer);
        }

        private static ulong ReadOpcode(NetDataReader reader)
        {
            return reader.GetULong();
        }
    }
}
