using System;
using System.IO;
using System.Web;
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

        public Url Decode(string body, string uri)
        {
            var url = new Url(uri);

            return DecodeN(body, DecipherSignature(body, url));
        }

        private Url DecodeN(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("n", out var n))
            {
                return url;
            }

            Console.WriteLine(n.ToString());
            var nCode = DecodeUtils.ExtractNCode(body, n.ToString());
            var nDecoded = _engine.Evaluate(nCode).ToString();
            Console.WriteLine(nDecoded);

            return url.SetQueryParam("n", nDecoded);
        }

        private Url DecipherSignature(string body, Url url)
        {
            if (!url.QueryParams.TryGetFirst("s", out var s))
            {
                return url;
            }

            var signatureParameter = url.QueryParams.FirstOrDefault("sp") as string ?? "signature";

            var sigCode = DecodeUtils.ExtractDecipher(body, s.ToString());
            var signatureValue = _engine.Evaluate(sigCode).ToString();

            return url.SetQueryParam(signatureParameter, signatureValue);
        }
    }
}