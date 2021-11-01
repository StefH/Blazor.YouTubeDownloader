using System;
using System.IO;
using System.Web;
using Jurassic;

namespace YouTubeUrlDecoder
{
    public class UrlDecoder
    {
        private readonly ScriptEngine _engine;

        public UrlDecoder()
        {
            _engine = new ScriptEngine();
        }

        public string Decode(string uri)
        {
            var body = File.ReadAllText(@"C:\Dev\GitHub\Blazor.YouTubeDownloader\Decode\base.js");

            string query = new Uri(uri).Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            var n = queryParams["n"];
            var nCode = DecodeUtils.ExtractNCode(body, n);
            var nDecoded = _engine.Evaluate(nCode).ToString();

            var sig = queryParams["sig"];
            var sigCode = DecodeUtils.ExtractDecipher(body, sig);
            var sigDecode = _engine.Evaluate(sigCode).ToString();

            return uri.Replace(n, nDecoded).Replace(sig, sigDecode);
        }
    }
}