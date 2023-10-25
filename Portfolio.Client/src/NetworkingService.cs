using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Portfolio.Protocol;

namespace Portfolio.Client
{
    public class NetworkingService : INetEventListener
    {
        private readonly Dictionary<ulong, Action<NetDataReader>> _packetHandlers = new();
        private readonly NetManager _manager;
        private readonly NetDataWriter _packetWriter = new();

        private NetPeer? _peer;

        public NetworkingService()
        {
            _manager = new NetManager(this);
            _manager.AutoRecycle = true;
        }

        public void Connect(string address, int port, string secret)
        {
            _manager.Start();
            _manager.Connect(address, port, secret);
        }

        public void Update()
        {
            _manager.PollEvents();
        }

        public void Stop()
        {
            _manager.Stop();
        }

        public void RegisterMessageHandler<T>(Action<T> handler) where T : IMessage, new()
        {
            var packet = new T();

            _packetHandlers[PacketHash.Get<T>()] = reader =>
            {
                Console.WriteLine($"Processing Packet: {typeof(T).Name}");

                packet.Deserialize(reader);

                handler.Invoke(packet);
            };
        }

        public void Send<T>(T message) where T : ICommand
        {
            if (_peer == null)
            {
                return;
            }

            _packetWriter.Reset();
            _packetWriter.Put(PacketHash.Get<T>());

            message.Serialize(_packetWriter);

            _peer.Send(_packetWriter, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("OnPeerConnected");

            _peer = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("OnPeerDisconnected");

            _peer = null;
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine("OnNetworkError");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine("OnNetworkReceive");

            if (_packetHandlers.TryGetValue(reader.GetULong(), out var handler))
            {
                handler.Invoke(reader);
            }
            else
            {
                Console.WriteLine("OnNetworkReceive: no packet handler");
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Console.WriteLine("OnNetworkLatencyUpdate");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine("OnConnectionRequest");
        }
    }
}
