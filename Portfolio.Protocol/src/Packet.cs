namespace Portfolio.Protocol
{
    public static class Packet
    {
        public static ulong GetId<T>()
        {
            return HashCache<T>.Id;
        }

        private static class HashCache<T>
        {
            public static readonly ulong Id;

            static HashCache()
            {
                ulong num1 = 14695981039346656037;

                foreach (ulong num2 in typeof(T).ToString())
                {
                    num1 = (num1 ^ num2) * 1099511628211UL;
                }

                Id = num1;
            }
        }
    }
}
