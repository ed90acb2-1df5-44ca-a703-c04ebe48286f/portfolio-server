using System;
using System.Collections.Generic;

namespace Portfolio.Common
{
    public static class TypeHash
    {
        private static readonly Dictionary<ulong, Type> _types = new();

        public static Type? Type(ulong opcode)
        {
            return _types.GetValueOrDefault(opcode);
        }

        public static ulong Hash<T>()
        {
            var opcode = HashCache<T>.Value;

            _types.TryAdd(opcode, typeof(T));

            return opcode;
        }

        private static class HashCache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static readonly ulong Value;

            static HashCache()
            {
                var num1 = 14695981039346656037;

                foreach (ulong num2 in typeof(T).ToString())
                {
                    num1 = (num1 ^ num2) * 1099511628211UL;
                }

                Value = num1;
            }
        }
    }
}
