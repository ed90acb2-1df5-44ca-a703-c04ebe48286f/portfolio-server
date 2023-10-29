using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using LiteNetLib;
using LiteNetLib.Utils;
using Portfolio.Protocol;

namespace Portfolio.Client
{
    public class NetworkConnection : INetEventListener
    {
        private readonly Dictionary<ulong, Action<int, NetDataReader>> _handlers = new();
        private readonly BufferWriter _buffer = new();
        private readonly NetManager _manager;

        private NetPeer? _peer;

        public NetworkConnection()
        {
            _manager = new NetManager(this);
            _manager.AutoRecycle = true;
        }

        public void Open(string address, int port, string secret)
        {
            _manager.Start();
            _peer = _manager.Connect(address, port, secret);
        }

        public void PollEvents()
        {
            _manager.PollEvents();
        }

        public void Close()
        {
            _manager.Stop();
        }

        public void RegisterHandler<TMessage>(Action<TMessage> handler) where TMessage : class, IMessage, new()
        {
            var packet = new TMessage();

            _handlers[PacketHash.Get<TMessage>()] = (peer, reader) =>
            {
                //Console.WriteLine($"Processing Packet: {typeof(TMessage).Name}");

                packet.MergeFrom(reader.GetRemainingBytes());

                handler.Invoke(packet);
            };
        }

        public void Send<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            if (_peer == null)
            {
                Console.WriteLine("Connecting...");
                return;
            }

            _buffer.Reset();
            _buffer.Write(PacketHash.Get<TMessage>());

            message.WriteTo(_buffer);

            _peer.Send(_buffer.AsSpan(), DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("OnPeerConnected");
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
            //Console.WriteLine("OnNetworkReceive");

            if (_handlers.TryGetValue(reader.GetULong(), out var handler))
            {
                handler.Invoke(peer.Id, reader);
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
