using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Protocol;
using Portfolio.Server.Handlers;

namespace Portfolio.Server
{
    public class NetworkingService : INetEventListener
    {
        private readonly Dictionary<int, NetPeer> _peers = new();
        private readonly Dictionary<ulong, Action<int, NetDataReader>> _packetHandlers = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NetworkingService> _logger;
        private readonly IOptions<ServerSettings> _options;
        private readonly NetManager _manager;
        private readonly NetDataWriter _packetWriter = new();

        public NetworkingService(IServiceProvider serviceProvider, ILogger<NetworkingService> logger, IOptions<ServerSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options;

            _manager = new NetManager(this);
            _manager.UseNativeSockets = options.Value.UseNativeSockets;
            _manager.AutoRecycle = true;
        }

        public void Start()
        {
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

        public void RegisterCommandHandler<TCommand, THandler>() where TCommand : struct, ICommand where THandler : IHandler<TCommand>
        {
            var command = new TCommand();

            _packetHandlers[PacketHash.Get<TCommand>()] = async (peerId, reader) =>
            {
                command.Deserialize(reader);

                using var scope = _serviceProvider.CreateScope();
                await scope.ServiceProvider.GetRequiredService<THandler>().Handle(peerId, command);
            };
        }

        public void Kick(int peerId)
        {
            if (_peers.ContainsKey(peerId) == false)
            {
                return;
            }

            _peers[peerId].Disconnect();
        }

        public void Send<TMessage>(int peerId, TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where TMessage : class, IMessage
        {
            if (_peers.ContainsKey(peerId) == false)
            {
                return;
            }

            lock (_packetWriter)
            {
                _packetWriter.Reset();
                _packetWriter.Put(PacketHash.Get<TMessage>());

                message.Serialize(_packetWriter);

                _peers[peerId].Send(_packetWriter, deliveryMethod);
            }
        }

        public void Broadcast<TMessage>(TMessage message, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where TMessage : class, IMessage
        {
            foreach (var peer in _peers.Values)
            {
                Send(peer.Id, message, deliveryMethod);
            }
        }

        public void BroadcastExcept<TMessage>(TMessage message, int peerId, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where TMessage : class, IMessage
        {
            foreach (var peer in _peers.Values)
            {
                if (peer.Id == peerId)
                {
                    continue;
                }

                Send(peer.Id, message, deliveryMethod);
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

            if (_packetHandlers.TryGetValue(reader.GetULong(), out var handler))
            {
                handler.Invoke(peer.Id, reader);
            }
            else
            {
                _logger.LogDebug("OnNetworkReceive: no packet handler");
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
