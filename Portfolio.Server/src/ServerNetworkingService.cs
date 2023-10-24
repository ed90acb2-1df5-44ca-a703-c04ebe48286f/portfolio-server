using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Net;
using Portfolio.Protocol;
using Portfolio.Protocol.Packets;

namespace Portfolio.Server
{
    public class ServerNetworkingService : INetEventListener
    {
        private readonly Dictionary<int, NetPeer> _peers = new();
        private readonly Dictionary<ulong, Action<IPacketReader>> _packetHandlers = new();
        private readonly ILogger<ServerNetworkingService> _logger;
        private readonly IOptions<ServerSettings> _options;
        private readonly NetManager _manager;

        private readonly LiteNetLibPacketReader _packetReader = new();
        private readonly LiteNetLibPacketWriter _packetWriter = new(new NetDataWriter());

        public ServerNetworkingService(ILogger<ServerNetworkingService> logger, IOptions<ServerSettings> options)
        {
            _logger = logger;
            _options = options;
            _manager = new NetManager(this);
            _manager.UseNativeSockets = true;
            _manager.AutoRecycle = true;
        }

        public void Start()
        {
            _manager.Start(_options.Value.Port);
            _logger.LogInformation($"Server started at port: {_manager.LocalPort}");
            _logger.LogInformation(_options.Value.Secret);
        }

        public void Update()
        {
            _manager.PollEvents();
        }

        public void Stop()
        {
            _manager.Stop();
        }

        public void RegisterPacketHandler<T>(Action<T> handler) where T : IPacket, new()
        {
            var packet = new T();

            _packetHandlers[Packet.GetId<T>()] = reader =>
            {
                _logger.LogDebug($"Processing Packet: {typeof(T)}");
                packet.Deserialize(reader);
                handler.Invoke(packet);
            };
        }

        public void Send<T>(T message, int peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, IPacket
        {
            _packetWriter.Reset();
            _packetWriter.WriteULong(Packet.GetId<T>());

            message.Serialize(_packetWriter);

            _peers[peerId].Send(_packetWriter.Writer, deliveryMethod);
        }

        public void Broadcast<T>(T message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, IPacket
        {
            foreach (var peer in _peers.Values)
            {
                Send(message, peer.Id, deliveryMethod);
            }
        }

        public void BroadcastExcept<T>(T message, int peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, IPacket
        {
            foreach (var peer in _peers.Values)
            {
                if (peer.Id == peerId)
                {
                    continue;
                }

                Send(message, peer.Id, deliveryMethod);
            }
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
            _logger.LogDebug("OnNetworkReceive");

            _packetReader.Reader = reader;
            _packetHandlers[_packetReader.ReadULong()].Invoke(_packetReader);
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
