using System.Runtime.InteropServices;

namespace Portfolio.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Query
    {
        public readonly Bitmask255 RequiredMask;
        public readonly Bitmask255 OptionalMask;
        public readonly Bitmask255 ExcludedMask;

        public Query(Bitmask255 requiredMask, Bitmask255 optionalMask, Bitmask255 excludedMask)
        {
            RequiredMask = requiredMask;
            OptionalMask = optionalMask;
            ExcludedMask = excludedMask;
        }
    }
}