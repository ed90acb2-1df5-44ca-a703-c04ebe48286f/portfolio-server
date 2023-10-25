using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Portfolio.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Bitmask255
    {
        private const int SubmaskLastBitPosition = sizeof(uint) * 8 - 1;
        private const int SubmaskBitshift = 5; // 2 ^ 5 = 32

        private readonly uint _mask0;
        private readonly uint _mask1;
        private readonly uint _mask2;
        private readonly uint _mask3;
        private readonly uint _mask4;
        private readonly uint _mask5;
        private readonly uint _mask6;
        private readonly uint _mask7;

        private Bitmask255(uint mask0, uint mask1, uint mask2, uint mask3, uint mask4, uint mask5, uint mask6, uint mask7)
        {
            _mask0 = mask0;
            _mask1 = mask1;
            _mask2 = mask2;
            _mask3 = mask3;
            _mask4 = mask4;
            _mask5 = mask5;
            _mask6 = mask6;
            _mask7 = mask7;
        }

        [Pure]
        public Bitmask255 Set(int index)
        {
            var maskIndex = index >> SubmaskBitshift;
            var mask = 1u << (index & SubmaskLastBitPosition);

            var mask0 = _mask0;
            var mask1 = _mask1;
            var mask2 = _mask2;
            var mask3 = _mask3;
            var mask4 = _mask4;
            var mask5 = _mask5;
            var mask6 = _mask6;
            var mask7 = _mask7;

            switch (maskIndex)
            {
                case 0:
                    mask0 |= mask;
                    break;
                case 1:
                    mask1 |= mask;
                    break;
                case 2:
                    mask2 |= mask;
                    break;
                case 3:
                    mask3 |= mask;
                    break;
                case 4:
                    mask4 |= mask;
                    break;
                case 5:
                    mask5 |= mask;
                    break;
                case 6:
                    mask6 |= mask;
                    break;
                case 7:
                    mask7 |= mask;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            return new Bitmask255(mask0, mask1, mask2, mask3, mask4, mask5, mask6, mask7);
        }

        [Pure]
        public Bitmask255 Unset(int index)
        {
            var maskIndex = index >> SubmaskBitshift;
            var mask = 1u << (index & SubmaskLastBitPosition);

            var mask0 = _mask0;
            var mask1 = _mask1;
            var mask2 = _mask2;
            var mask3 = _mask3;
            var mask4 = _mask4;
            var mask5 = _mask5;
            var mask6 = _mask6;
            var mask7 = _mask7;

            switch (maskIndex)
            {
                case 0:
                    mask0 &= ~mask;
                    break;
                case 1:
                    mask1 &= ~mask;
                    break;
                case 2:
                    mask2 &= ~mask;
                    break;
                case 3:
                    mask3 &= ~mask;
                    break;
                case 4:
                    mask4 &= ~mask;
                    break;
                case 5:
                    mask5 &= ~mask;
                    break;
                case 6:
                    mask6 &= ~mask;
                    break;
                case 7:
                    mask7 &= ~mask;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            return new Bitmask255(mask0, mask1, mask2, mask3, mask4, mask5, mask6, mask7);
        }

        public bool IsSet(int index)
        {
            var maskIndex = index >> SubmaskBitshift;
            var mask = 1 << (index & SubmaskLastBitPosition);

            return maskIndex switch
            {
                0 => (_mask0 & mask) != 0,
                1 => (_mask1 & mask) != 0,
                2 => (_mask2 & mask) != 0,
                3 => (_mask3 & mask) != 0,
                4 => (_mask4 & mask) != 0,
                5 => (_mask5 & mask) != 0,
                6 => (_mask6 & mask) != 0,
                7 => (_mask7 & mask) != 0,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        public bool IsEmpty()
        {
            return _mask0 == 0 &&
                   _mask1 == 0 &&
                   _mask2 == 0 &&
                   _mask3 == 0 &&
                   _mask4 == 0 &&
                   _mask5 == 0 &&
                   _mask6 == 0 &&
                   _mask7 == 0;
        }

        public bool ContainsAll(in Bitmask255 other)
        {
            return (_mask0 & other._mask0) == other._mask0 &&
                   (_mask1 & other._mask1) == other._mask1 &&
                   (_mask2 & other._mask2) == other._mask2 &&
                   (_mask3 & other._mask3) == other._mask3 &&
                   (_mask4 & other._mask4) == other._mask4 &&
                   (_mask5 & other._mask5) == other._mask5 &&
                   (_mask6 & other._mask6) == other._mask6 &&
                   (_mask7 & other._mask7) == other._mask7;
        }

        public bool ContainsAny(in Bitmask255 other)
        {
            return (other._mask0 == 0 || (_mask0 & other._mask0) > 0) &&
                   (other._mask1 == 0 || (_mask1 & other._mask1) > 0) &&
                   (other._mask2 == 0 || (_mask2 & other._mask2) > 0) &&
                   (other._mask3 == 0 || (_mask3 & other._mask3) > 0) &&
                   (other._mask4 == 0 || (_mask4 & other._mask4) > 0) &&
                   (other._mask5 == 0 || (_mask5 & other._mask5) > 0) &&
                   (other._mask6 == 0 || (_mask6 & other._mask6) > 0) &&
                   (other._mask7 == 0 || (_mask7 & other._mask7) > 0);
        }
    }
}
