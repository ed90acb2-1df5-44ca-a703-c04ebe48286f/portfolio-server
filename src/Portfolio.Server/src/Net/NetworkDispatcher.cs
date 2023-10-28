using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Protocol;
using Portfolio.Server.Settings;

namespace Portfolio.Server.Net
{
    public class NetworkDispatcher : INetEventListener
    {
        private readonly Dictionary<ulong, Action<NetPeer, NetDataReader>> _handlers = new();
        private readonly Dictionary<Player, NetPeer> _peers = new();
        private readonly BufferWriter _buffer = new();
        private readonly NetManager _manager;

        private readonly ILogger<NetworkDispatcher> _logger;
        private readonly IOptions<ServerSettings> _options;

        public NetworkDispatcher(ILogger<NetworkDispatcher> logger, IOptions<ServerSettings> options)
        {
            _logger = logger;
            _options = options;

            _manager = new NetManager(this);
            _manager.UseNativeSockets = options.Value.UseNativeSockets;
            _manager.AutoRecycle = true;
        }

        public void RegisterHandler<TPacket>(Action<Player, TPacket> handler) where TPacket : class, IMessage, new()
        {
            var packet = new TPacket();

            _handlers[PacketHash.Get<TPacket>()] = (peer, reader) =>
            {
                packet.MergeFrom(reader.GetRemainingBytes());
                handler.Invoke((Player) peer.Tag, packet);
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

        public void Send<TMessage>(Player player, TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where TMessage : class, IMessage
        {
            lock (_buffer)
            {
                _buffer.Reset();
                _buffer.Write(PacketHash.Get<TMessage>());

                message.WriteTo(_buffer);

                _peers[player].Send(_buffer.Data(), DeliveryMethod.ReliableOrdered);
            }
        }

        public void Broadcast<TMessage>(TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
        }

        public void BroadcastExcept<TMessage>(TMessage message, int peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
        }

        public void Disconnect(Player player)
        {
            if (_peers.TryGetValue(player, out var peer))
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

            var player = new Player(peer.Id);
            peer.Tag = player;

            _peers.Add(player, peer);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            //_logger.LogDebug("OnNetworkReceive");

            if (_handlers.TryGetValue(reader.GetULong(), out var handler))
            {
                handler.Invoke(peer, reader);
            }
            else
            {
                _logger.LogWarning("Handler not found");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _logger.LogDebug("OnNetworkLatencyUpdate");

            var player = (Player) peer.Tag;
            player.Ping = latency;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.LogDebug("OnPeerDisconnected");

            var player = (Player) peer.Tag;

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
    }
}
