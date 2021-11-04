using Flurl;
using Jurassic;

namespace YoutubeExplode.Extensions.Utils
{
    internal class UrlDescrambler
    {
        private readonly ScriptEngine _engine;
        private readonly (string code, string argumentName) _nCode;
        private readonly (string code, string argumentName) _sCode;

        public UrlDescrambler(string playerSource)
        {
            _engine = new ScriptEngine();

            _nCode = DecodeUtils.ExtractNCode(playerSource);
            _sCode = DecodeUtils.ExtractDecipher(playerSource);
        }

        public Url Decode(Url url)
        {
            return DecodeN(DecipherSignature(url));
        }

        private Url DecodeN(Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            var nDecoded = Evaluate(_nCode, n);

            return url.SetQueryParam("n", nDecoded);
        }

        private Url DecipherSignature(Url url)
        {
            if (!url.QueryParams.TryGetFirst("s", out var s))
            {
                return url;
            }

            var signatureParameter = url.QueryParams.FirstOrDefault("sp") as string ?? "signature";
            var signatureValue = Evaluate(_sCode, s);

            return url.SetQueryParam(signatureParameter, signatureValue);
        }

        private string Evaluate((string code, string argumentName) x, object argumentValue)
        {
            var finalCode = $"const {x.argumentName} = '{argumentValue}';{x.code}";

            return _engine.Evaluate(finalCode).ToString();
        }
    }
}