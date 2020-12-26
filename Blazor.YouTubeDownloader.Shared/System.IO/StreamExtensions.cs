using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    /// <summary>
    /// Provides CopyToAsync method for operating on <see cref="Stream"/> instances.
    /// </summary>
	public static class StreamExtensions
    {
        private const int DefaultBufferSize = 81920;

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another stream and reports the progress.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to copy from.</param>
        /// <param name="sourceLength">The length of the source stream, if known - used for progress reporting.</param>
        /// <param name="destination">The <see cref="Stream"/> to which the contents of the current stream will be copied.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
        /// <param name="progress">An async function for reporting progress.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task representing the operation</returns>
        public static async Task CopyToAsync(
            this Stream source,
            long sourceLength,
            Stream destination,
            int bufferSize,
            Func<StreamCopyProgressInfo, Task> progress,
            CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, $"{nameof(bufferSize)} has to be greater than zero");
            }

            if (!source.CanRead)
            {
                throw new ArgumentException($"{nameof(source)} is not readable.", nameof(source));
            }

            if (!destination.CanWrite)
            {
                throw new ArgumentException($"{nameof(destination)} is not writable.", nameof(source));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (sourceLength <= 0 && source.CanSeek)
            {
                sourceLength = source.Length;
            }

            using var buffer = PooledBuffer.ForStream(bufferSize);

            long totalBytesCopied = 0L;
            int bytesRead;
            do
            {
                bytesRead = await source.CopyBufferedToAsync(destination, buffer.Array, cancellationToken);
                totalBytesCopied += bytesRead;

                await progress(new StreamCopyProgressInfo { BytesRead = bytesRead, TotalBytesRead = totalBytesCopied, SourceLength = sourceLength });
            } while (!cancellationToken.IsCancellationRequested && bytesRead > 0);
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another stream and reports the progress.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to copy from.</param>
        /// <param name="sourceLength">The length of the source stream, if known - used for progress reporting.</param>
        /// <param name="destination">The <see cref="Stream"/> to which the contents of the current stream will be copied.</param>
        /// <param name="progress">An async function for reporting progress.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>A task representing the operation</returns>
        public static Task CopyToAsync(
            this Stream source,
            long sourceLength,
            Stream destination,
            Func<StreamCopyProgressInfo, Task> progress,
            CancellationToken cancellationToken = default
        ) => CopyToAsync(source, sourceLength, destination, DefaultBufferSize, progress, cancellationToken);

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another stream and reports the progress.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to copy from</param>
        /// <param name="destination">The <see cref="Stream"/> to which the contents of the current stream will be copied.</param>
        /// <param name="progress">An async function for reporting progress.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        public static Task CopyToAsync(
            this Stream source,
            Stream destination,
            Func<StreamCopyProgressInfo, Task> progress,
            CancellationToken cancellationToken = default
        ) => CopyToAsync(source, 0L, destination, DefaultBufferSize, progress, cancellationToken);

        private static async Task<int> CopyBufferedToAsync(this Stream source, Stream destination, byte[] buffer, CancellationToken cancellationToken = default)
        {
            var bytesRead = await source.ReadAsync(buffer, cancellationToken);
            await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);

            return bytesRead;
        }
    }
}