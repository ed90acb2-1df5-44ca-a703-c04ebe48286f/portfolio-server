using System;
using System.Collections.Generic;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;

namespace Portfolio.Protocol
{
    public class Opcodes
    {
        private static readonly Dictionary<Type, Opcode> _dictionary = new()
        {
            {typeof(LoginCommand), Opcode.LoginCommand},
            {typeof(LoginMessage), Opcode.LoginMessage},
            {typeof(BroadcastMessage), Opcode.BroadcastMessage},
        };

        public static Opcode Get<T>()
        {
            return _dictionary[typeof(T)];
        }
    }
}
