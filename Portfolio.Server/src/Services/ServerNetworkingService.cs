using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Protocol;

namespace Portfolio.Server.Services
{
    public class ServerNetworkingService : INetEventListener
    {
        private readonly Dictionary<ulong, Action<int, NetDataReader>> _handlers = new();
        private readonly Dictionary<int, NetPeer> _peers = new();
        private readonly BufferWriter _buffer = new();
        private readonly NetManager _manager;

        private readonly ILogger<ServerNetworkingService> _logger;
        private readonly IOptions<ServerSettings> _options;

        public ServerNetworkingService(ILogger<ServerNetworkingService> logger, IOptions<ServerSettings> options)
        {
            _logger = logger;
            _options = options;

            _manager = new NetManager(this);
            _manager.UseNativeSockets = options.Value.UseNativeSockets;
            _manager.AutoRecycle = true;
        }

        public void RegisterHandler<TPacket>(Action<int, TPacket> handler) where TPacket : class, IMessage, new()
        {
            var packet = new TPacket();

            _handlers[PacketHash.Get<TPacket>()] = (peerId, reader) =>
            {
                packet.MergeFrom(reader.GetRemainingBytes());
                handler.Invoke(peerId, packet);
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

        public void Send<TMessage>(int peerId, TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where TMessage : class, IMessage
        {
            lock (_buffer)
            {
                _buffer.Reset();
                _buffer.Write(PacketHash.Get<TMessage>());

                message.WriteTo(_buffer);

                _peers[peerId].Send(_buffer.Data(), DeliveryMethod.ReliableOrdered);
            }
        }

        public void Broadcast<TMessage>(TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
        }

        public void BroadcastExcept<TMessage>(TMessage message, int peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
        }

        public void Disconnect(int peerId)
        {
            if (_peers.ContainsKey(peerId) == false)
            {
                return;
            }

            _peers[peerId].Disconnect();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _logger.LogDebug("OnPeerConnected");

            _peers.Add(peer.Id, peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.LogDebug("OnPeerDisconnected");

            _peers.Remove(peer.Id);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            _logger.LogDebug("OnNetworkError");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            //_logger.LogDebug("OnNetworkReceive");

            if (_handlers.TryGetValue(reader.GetULong(), out var handler))
            {
                handler.Invoke(peer.Id, reader);
            }
            else
            {
                _logger.LogDebug("Handler not found");
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            _logger.LogDebug("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _logger.LogDebug("OnNetworkLatencyUpdate");
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
    }
}
