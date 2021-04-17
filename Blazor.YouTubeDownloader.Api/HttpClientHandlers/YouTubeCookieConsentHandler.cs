using System.Net;
using System.Net.Http;

namespace Blazor.YouTubeDownloader.Api.HttpClientHandlers
{
    /// <summary>
    /// https://github.com/Tyrrrz/YoutubeExplode/issues/529
    /// https://github.com/omansak/libvideo/issues/201
    /// </summary>
    public class YouTubeCookieConsentHandler : HttpClientHandler
    {
        public YouTubeCookieConsentHandler()
        {
            UseCookies = true;
            CookieContainer = new CookieContainer();
            CookieContainer.Add(new Cookie("CONSENT", "YES+cb", "/", "youtube.com"));
        }
    }
}