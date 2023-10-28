using System;
using System.Buffers;

namespace Portfolio.Protocol
{
    public class BufferWriter : IBufferWriter<byte>
    {
        private byte[] _buffer;
        private int _position;

        public BufferWriter(int capacity = 1024)
        {
            _buffer = new byte[capacity];
            _position = 0;
        }

        public void Advance(int count)
        {
            EnsureCapacity(count);
            _position += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsMemory(_position);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _buffer.AsSpan(_position);
        }

        public void Write(ulong value)
        {
            Span<byte> valueBytes = stackalloc byte[sizeof(ulong)];
            BitConverter.TryWriteBytes(valueBytes, value);
            Write(valueBytes);
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            EnsureCapacity(data.Length);
            data.CopyTo(_buffer.AsSpan(_position));
            _position += data.Length;
        }

        public void Reset()
        {
            _position = 0;
        }

        public ReadOnlySpan<byte> Data()
        {
            return _buffer.AsSpan(0, _position);
        }

        private void EnsureCapacity(int sizeHint)
        {
            if (_position + sizeHint <= _buffer.Length)
            {
                return;
            }

            var newSize = Math.Max(_position + sizeHint, _buffer.Length * 2);
            Array.Resize(ref _buffer, newSize);
        }
    }
}
