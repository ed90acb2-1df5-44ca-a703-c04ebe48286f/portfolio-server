using System.Runtime.InteropServices;
using Portfolio.Entities.Collections;

namespace Portfolio.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Query
    {
        public readonly Bitmask255 RequiredMask;
        public readonly Bitmask255 ExcludedMask;

        public Query(Bitmask255 requiredMask, Bitmask255 excludedMask)
        {
            RequiredMask = requiredMask;
            ExcludedMask = excludedMask;
        }
    }
}
