using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Portfolio.Net;
using Portfolio.Protocol;
using Portfolio.Protocol.Packets;

namespace Portfolio.Client
{
    public class ClientNetworkingService : INetEventListener
    {
        private readonly Dictionary<ulong, Action<IPacketReader>> _packetHandlers = new();
        private readonly NetManager _manager;

        private readonly LiteNetLibPacketReader _packetReader = new();
        private readonly LiteNetLibPacketWriter _packetWriter = new(new NetDataWriter());

        private NetPeer? _peer;

        public ClientNetworkingService()
        {
            _manager = new NetManager(this);
            _manager.UseNativeSockets = true;
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

        public void RegisterPacketHandler<T>(Action<T> handler) where T : IPacket, new()
        {
            var packet = new T();

            _packetHandlers[Packet.GetId<T>()] = reader =>
            {
                Console.WriteLine($"Processing Packet: {typeof(T)}");

                packet.Deserialize(reader);
                handler.Invoke(packet);
            };
        }

        public void Send<T>(T message) where T : class, IPacket
        {
            if (_peer == null)
            {
                Console.WriteLine("Connecting...");
                return;
            }

            _packetWriter.Reset();
            _packetWriter.WriteULong(Packet.GetId<T>());

            message.Serialize(_packetWriter);

            _peer.Send(_packetWriter.Data(), DeliveryMethod.ReliableOrdered);
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

            _packetReader.Reader = reader;
            _packetHandlers[_packetReader.ReadULong()].Invoke(_packetReader);
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
