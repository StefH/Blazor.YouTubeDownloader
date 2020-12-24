using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorDownloadFile
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Converts a stream into a byte array
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            var streamLength = (int)stream.Length;
            var data = new byte[streamLength];
            stream.Position = 0;
            stream.Read(data, 0, streamLength);
            return data;
        }
        /// <summary>
        /// Converts a stream into a byte array
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns></returns>
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
        {
            var streamLength = (int)stream.Length;
            var data = new byte[streamLength];
            stream.Position = 0;
            await stream.ReadAsync(data, 0, streamLength);
            return data;
        }
        /// <summary>
        /// Converts a stream into a byte array
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken)
        {
            var streamLength = (int)stream.Length;
            var data = new byte[streamLength];
            stream.Position = 0;
            await stream.ReadAsync(data, 0, streamLength, cancellationToken);
            return data;
        }
    }
}
