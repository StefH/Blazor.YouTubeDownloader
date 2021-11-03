using System.Reflection;
using Stef.Validation;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.Extensions.Extensions
{
    public static class StreamInfoExtensions
    {
        public static void Fix(this IStreamInfo streamInfo)
        {
            Guard.NotNull(streamInfo, nameof(streamInfo));

            if (string.IsNullOrEmpty(streamInfo.Url))
            {
                return;
            }

            var fixedUrl = UrlDescrambler.Fix(streamInfo.Url).ToString();

            var field = streamInfo.GetType().GetField("<Url>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(streamInfo, fixedUrl);
        }
    }
}