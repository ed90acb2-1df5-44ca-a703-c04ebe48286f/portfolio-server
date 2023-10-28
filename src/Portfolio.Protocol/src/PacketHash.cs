namespace Portfolio.Protocol
{
    public static class PacketHash
    {
        public static ulong Get<T>()
        {
            return HashCache<T>.Value;
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
