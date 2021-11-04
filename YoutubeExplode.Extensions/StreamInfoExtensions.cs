using System.Reflection;
using Stef.Validation;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.Extensions
{
    internal static class StreamInfoExtensions
    {
        public static void UpdateUrl(this IStreamInfo streamInfo, string url)
        {
            Guard.NotNull(streamInfo, nameof(streamInfo));

            if (string.IsNullOrEmpty(streamInfo.Url))
            {
                return;
            }

            var field = streamInfo.GetType().GetField("<Url>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(streamInfo, url);
        }
    }
}