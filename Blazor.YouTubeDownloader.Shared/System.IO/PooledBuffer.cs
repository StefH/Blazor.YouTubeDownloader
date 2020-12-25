using System.Buffers;

namespace System.IO
{
    // Copied from https://github.com/Tyrrrz/YoutubeExplode
    internal readonly struct PooledBuffer<T> : IDisposable
    {
        public T[] Array { get; }

        public PooledBuffer(int minimumLength) => Array = ArrayPool<T>.Shared.Rent(minimumLength);

        public void Dispose() => ArrayPool<T>.Shared.Return(Array);
    }

    // Based on https://github.com/Tyrrrz/YoutubeExplode
    internal static class PooledBuffer
    {
        public static PooledBuffer<byte> ForStream(int length = 81920) => new PooledBuffer<byte>(length);
    }
}