using System.Collections;

namespace Portfolio.Entities
{
    public static class CollectionExtensions
    {
        public static bool IndexInBounds(this ICollection collection, int index)
        {
            return index >= 0 && index < collection.Count;
        }
    }
}