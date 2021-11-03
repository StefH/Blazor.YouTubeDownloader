using Flurl;
using Jurassic;
using YoutubeExplode.Extensions.JavaScript;

namespace YoutubeExplode.Extensions
{
    public static class UrlDescrambler
    {
        private static readonly ScriptEngine _engine;

        static UrlDescrambler()
        {
            _engine = new ScriptEngine();
        }

        public static Url Fix(Url url)
        {
            return DecodeN(DecipherSignature(url));
        }

        public static Url DecodeN(Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            // Option 2
            var nDecoded = Evaluate(N.Code, "n__", n);

            return url.SetQueryParam("n", nDecoded);
        }


        private static Url DecipherSignature(Url url)
        {
            if (!url.QueryParams.TryGetFirst("s", out var s))
            {
                return url;
            }

            var signatureParameter = url.QueryParams.FirstOrDefault("sp") as string ?? "signature";

            // Option 2
            var signatureValue = Evaluate(S.Code, "s__", s);

            return url.SetQueryParam(signatureParameter, signatureValue);
        }


        private static string Evaluate(string code, string argumentName, object argumentValue)
        {
            var finalCode = $"const {argumentName} = '{argumentValue}';{code}";

            return _engine.Evaluate(finalCode).ToString();
        }
    }
}