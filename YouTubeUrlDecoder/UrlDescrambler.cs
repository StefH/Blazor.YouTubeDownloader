using System;
using Flurl;
using Jurassic;

namespace YouTubeUrlDecoder
{
    public class UrlDescrambler
    {
        private readonly ScriptEngine _engine;

        public UrlDescrambler()
        {
            _engine = new ScriptEngine();
        }

        public Url Decode(string body, Url url)
        {
            return DecodeN(body, DecipherSignature(body, url));
        }

        private Url DecodeN(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            var (code, argumentName) = DecodeUtils.ExtractNCode(body);
            var nDecoded = Evaluate(code, argumentName, n);

            return url.SetQueryParam("n", nDecoded);
        }

        private Url DecipherSignature(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("s", out var s))
            {
                return url;
            }

            var signatureParameter = url.QueryParams.FirstOrDefault("sp") as string ?? "signature";

            var (code, argumentName) = DecodeUtils.ExtractDecipher(body);
            var signatureValue = Evaluate(code, argumentName, s);

            return url.SetQueryParam(signatureParameter, signatureValue);
        }

        private string Evaluate(string code, string argumentName, object argumentValue)
        {
            var finalCode = $"const {argumentName} = '{argumentValue}';{code}";
            // Console.WriteLine(finalCode);
            return _engine.Evaluate(finalCode).ToString();
        }
    }
}