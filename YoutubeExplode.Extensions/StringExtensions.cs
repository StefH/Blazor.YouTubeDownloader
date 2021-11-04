using System;

namespace YoutubeExplode.Extensions
{
    /// <summary>
    /// Based on https://github.com/fent/node-ytdl-core/pull/1022
    /// </summary>
    internal static class StringExtensions
    {
        public static string Between(this string body, string value1, string value2)
        {
            int startindex = body.IndexOf(value1) + value1.Length;
            int endindex = body.IndexOf(value2, startindex);
            return body.Substring(startindex, endindex - startindex);
        }

        public static string CutAfterJSON(this string mixedJson)
        {
            char open;
            char close;
            if (mixedJson[0] == '[')
            {
                open = '[';
                close = ']';
            }
            else if (mixedJson[0] == '{')
            {
                open = '{';
                close = '}';
            }
            else
            {
                throw new Exception($"Can't cut unsupported JSON (need to begin with [ or {{ ) but got: {mixedJson[0]}");
            }

            // States if the loop is currently in a string
            bool isString = false;

            // States if the current character is treated as escaped or not
            bool isEscaped = false;

            // Current open brackets to be closed
            int counter = 0;

            for (int i = 0; i < mixedJson.Length; i++)
            {
                // Toggle the isString boolean when leaving/entering string
                if (mixedJson[i] == '"' && !isEscaped)
                {
                    isString = !isString;
                    continue;
                }

                // Toggle the isEscaped boolean for every backslash
                // Reset for every regular character
                isEscaped = mixedJson[i] == '\\' && !isEscaped;

                if (isString) continue;

                if (mixedJson[i] == open)
                {
                    counter++;
                }
                else if (mixedJson[i] == close)
                {
                    counter--;
                }

                // All brackets have been closed, thus end of JSON is reached
                if (counter == 0)
                {
                    // Return the cut JSON
                    return mixedJson.Substring(0, i + 1);
                }
            }

            // We ran through the whole string and ended up with an unclosed bracket
            throw new Exception("Can't cut unsupported JSON (no matching closing bracket found)");
        }
    }
}
