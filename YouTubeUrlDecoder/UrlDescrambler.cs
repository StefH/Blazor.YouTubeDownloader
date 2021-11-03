using Flurl;
using Jurassic;
using YouTubeUrlDecoder.JavaScript;

namespace YouTubeUrlDecoder
{
    public class UrlDescrambler2
    {
        private readonly ScriptEngine _engine;

        public UrlDescrambler2()
        {
            _engine = new ScriptEngine();
        }

        public Url Fix(Url url)
        {
            return DecodeN(DecipherSignature(url));
        }

        public Url Decode(string code, Url url)
        {
            //var (nCode, nArgumentName) = DecodeUtils.ExtractNCode(body);
            //var (sCode, sArgumentName) = DecodeUtils.ExtractDecipher(body);

            return DecodeN(code, DecipherSignature(code, url));
        }

        public Url DecodeN(Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            // Option 2
            var nDecoded = Evaluate(N.Code, "n__", n); // Hardcoded...

            return url.SetQueryParam("n", nDecoded);
        }

        private Url DecodeN(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            // Option 1
            var (code, argumentName) = DecodeUtils.ExtractNCode(body);
            var nDecoded = Evaluate(code, argumentName, n);

            return url.SetQueryParam("n", nDecoded);
        }

        private Url DecipherSignature(Url url)
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

        private Url DecipherSignature(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("s", out var s))
            {
                return url;
            }

            var signatureParameter = url.QueryParams.FirstOrDefault("sp") as string ?? "signature";

            // Option 1
            var (code, argumentName) = DecodeUtils.ExtractDecipher(body);
            var signatureValue = Evaluate(code, argumentName, s);

            return url.SetQueryParam(signatureParameter, signatureValue);
        }

        private string Evaluate(string code, string argumentName, object argumentValue)
        {
            var finalCode = $"const {argumentName} = '{argumentValue}';{code}";

            return _engine.Evaluate(finalCode).ToString();
        }
    }
}