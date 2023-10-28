using System;
using System.Diagnostics.Contracts;

namespace Portfolio.Entities
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public static readonly Entity Empty = new(-1, -1);

        public readonly int Index;

        private readonly int _version;

        public Entity(int index)
        {
            Index = index;
            _version = 0;
        }

        private Entity(int index, int version)
        {
            Index = index;
            _version = version;
        }

        [Pure]
        public Entity WithNewVersion()
        {
            return new Entity(Index, _version + 1);
        }

        public bool Equals(Entity other)
        {
            return Index == other.Index && _version == other._version;
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, _version);
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !left.Equals(right);
        }
    }
}
