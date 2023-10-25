using System;
using System.Runtime.InteropServices;

namespace Portfolio.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Bitmask
    {
        public const byte BitsInByte = 8;

        private readonly byte[] _bytes;

        public Bitmask(int capacity)
        {
            _bytes = new byte[capacity];
        }

        public bool IsSet(int flagIndex)
        {
            if (flagIndex < 0 || flagIndex >= _bytes.Length * BitsInByte)
            {
                throw new ArgumentOutOfRangeException(nameof(flagIndex), "Flag index is out of range.");
            }

            var byteIndex = flagIndex / BitsInByte;
            var bitIndex = flagIndex % BitsInByte;

            var mask = (byte) (1 << bitIndex);
            return (_bytes[byteIndex] & mask) != 0;
        }

        public bool IsEmpty()
        {
            for (var i = 0; i < _bytes.Length; i++)
            {
                if (_bytes[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            for (var i = 0; i < _bytes.Length; i++)
            {
                _bytes[i] = 0;
            }
        }

        public void Set(int index)
        {
            if (index < 0 || index >= _bytes.Length * BitsInByte)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Flag index is out of range.");
            }

            var byteIndex = index / BitsInByte;
            var bitIndex = index % BitsInByte;

            var mask = (byte) (1 << bitIndex);
            _bytes[byteIndex] |= mask;
        }

        public void Unset(int index)
        {
            if (index < 0 || index >= _bytes.Length * BitsInByte)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Flag index is out of range.");
            }

            var byteIndex = index / BitsInByte;
            var bitIndex = index % BitsInByte;

            var mask = (byte) (1 << bitIndex);
            _bytes[byteIndex] &= (byte) ~mask;
        }

        public bool ContainsAll(Bitmask other)
        {
            for (var i = 0; i < _bytes.Length; i++)
            {
                if ((_bytes[i] & other._bytes[i]) != other._bytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsAny(Bitmask other)
        {
            for (var i = 0; i < _bytes.Length; i++)
            {
                if ((_bytes[i] & other._bytes[i]) != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
