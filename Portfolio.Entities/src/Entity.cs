namespace Portfolio.Entities
{
    public readonly struct Entity
    {
        public readonly int Index;
        public readonly Bitmask255 Mask;

        public Entity(int index)
        {
            Index = index;
            Mask = new Bitmask255();
        }

        private Entity(int index, Bitmask255 mask)
        {
            Index = index;
            Mask = mask;
        }

        public Entity WithMask(Bitmask255 mask)
        {
            return new Entity(Index, mask);
        }
    }
}
