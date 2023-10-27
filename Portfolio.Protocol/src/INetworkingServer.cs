using System;

namespace Portfolio.Protocol
{
    /// <summary>
    /// Basic server networking operations.
    /// </summary>
    public interface INetworkingServer
    {
        /// <summary>
        /// Register delegate that will be called when server receive and decode client command of specified type.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TCommand"></typeparam>
        void RegisterCommandHandler<TCommand>(Action<int, TCommand> handler);

        /// <summary>
        /// Start listening for network events.
        /// </summary>
        void Start();

        /// <summary>
        /// Send message to the peer.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="message"></param>
        /// <param name="deliveryMethod"></param>
        /// <typeparam name="TMessage"></typeparam>
        void Send<TMessage>(int peer, TMessage message);

        /// <summary>
        /// Send message to all connected peers, except specified.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="message"></param>
        /// <param name="deliveryMethod"></param>
        /// <typeparam name="TMessage"></typeparam>
        void SendToOthers<TMessage>(int peer, TMessage message);

        /// <summary>
        /// Send message to all connected peers.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deliveryMethod"></param>
        /// <typeparam name="TMessage"></typeparam>
        void SendToAll<TMessage>(TMessage message);
    }
}
