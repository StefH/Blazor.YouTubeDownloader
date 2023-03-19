using Azure.Core;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Blazor.YouTubeDownloader.Api.Extensions;

internal static class HttpRequestDataExtensions
{
    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(this HttpRequestData req, T value)
    {
        var response = req.CreateResponse();
        response.Headers.Add(HttpHeader.Names.ContentType, HttpHeader.Common.JsonContentType.Value);

        await response.WriteStringAsync(JsonConvert.SerializeObject(value));

        return response;
    }

    public static HttpResponseData CreateStreamResponse(this HttpRequestData req, Stream bodyAsStream)
    {
        var response = req.CreateResponse();
        response.Headers.Add(HttpHeader.Names.ContentType, HttpHeader.Common.OctetStreamContentType.Value);
        response.Body = bodyAsStream;

        return response;
    }
}