using YoutubeExplode.Extensions.Extensions;

namespace YoutubeExplode.Extensions.Utils
{
    /// <summary>
    /// Based on https://github.com/fent/node-ytdl-core/pull/1022
    /// </summary>
    internal static class DecodeUtils
    {
        public static (string code, string argumentName) ExtractNCode(string body)
        {
            var argumentName = $"n__";
            var functionName = body.Between("&&(b=a.get(\"n\"))&&(b=", "(b)");
            var functionStart = $"{functionName}=function(a)";

            var ndx = body.IndexOf(functionStart);
            var subBody = body.Substring(ndx + functionStart.Length);

            return ($"var {functionStart}{subBody.CutAfterJSON()};{functionName}({argumentName});", argumentName);
        }

        public static (string code, string argumentName) ExtractDecipher(string body)
        {
            var argumentName = $"s__";
            var functionName = body.Between("a.set(\"alr\",\"yes\");c&&(c=", "(decodeURIComponent");
            var functionStart = $"{functionName}=function(a)";

            var ndx = body.IndexOf(functionStart);
            var subBody = body.Substring(ndx + functionStart.Length);

            var functionBody = $"var {functionStart}{subBody.CutAfterJSON()};";

            return ($"{ExtractManipulations(body, functionBody)};{functionBody};{functionName}({argumentName});", argumentName);
        }

        private static string ExtractManipulations(string body, string caller)
        {
            var functionName = caller.Between("a=a.split(\"\");", ".");
            if (string.IsNullOrEmpty(functionName))
            {
                return string.Empty;
            }

            var functionStart = $"var {functionName}={{";
            var ndx = body.IndexOf(functionStart);
            if (ndx < 0)
            {
                return string.Empty;
            }

            var subBody = body.Substring(ndx + functionStart.Length - 1);
            return $"var {functionName}={subBody.CutAfterJSON()}";
        }
    }
}
