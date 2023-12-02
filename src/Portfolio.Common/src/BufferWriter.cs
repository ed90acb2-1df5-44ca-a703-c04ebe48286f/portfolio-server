using System;
using System.Buffers;

namespace Portfolio.Common
{
    public class BufferWriter : IBufferWriter<byte>
    {
        public int Position;
        public byte[] Buffer;

        public BufferWriter(int capacity = 1024)
        {
            Buffer = new byte[capacity];
            Position = 0;
        }

        public void Advance(int count)
        {
            EnsureCapacity(count);
            Position += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return Buffer.AsMemory(Position);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return Buffer.AsSpan(Position);
        }

        public void Write(ulong value)
        {
            Span<byte> valueBytes = stackalloc byte[sizeof(ulong)];
            BitConverter.TryWriteBytes(valueBytes, value);
            Write(valueBytes);
        }

        public void Write(uint value)
        {
            Span<byte> valueBytes = stackalloc byte[sizeof(uint)];
            BitConverter.TryWriteBytes(valueBytes, value);
            Write(valueBytes);
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            EnsureCapacity(data.Length);
            data.CopyTo(Buffer.AsSpan(Position));
            Position += data.Length;
        }

        public void Reset()
        {
            Position = 0;
        }

        public ReadOnlySpan<byte> AsSpan()
        {
            return Buffer.AsSpan(0, Position);
        }

        private void EnsureCapacity(int sizeHint)
        {
            if (Position + sizeHint <= Buffer.Length)
            {
                return;
            }

            var newSize = Math.Max(Position + sizeHint, Buffer.Length * 2);
            Array.Resize(ref Buffer, newSize);
        }
    }
}
