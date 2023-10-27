using System;

namespace Portfolio.Protocol
{
    /// <summary>
    /// Basic client networking operations.
    /// </summary>
    public interface INetworkingClient
    {
        /// <summary>
        /// Register delegate that will be called when client receive and decode message of specified type.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TMessage"></typeparam>
        void RegisterMessageHandler<TMessage>(Action<int, TMessage> handler);

        /// <summary>
        /// Open network connection and start listening for events.
        /// </summary>
        void Connect();

        /// <summary>
        /// Send command to the server.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="deliveryMethod"></param>
        /// <typeparam name="TCommand"></typeparam>
        void Send<TCommand>(TCommand command);
    }
}
