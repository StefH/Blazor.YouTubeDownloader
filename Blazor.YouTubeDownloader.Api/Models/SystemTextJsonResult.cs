using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.YouTubeDownloader.Api.Models
{
    public class SystemTextJsonResult : ContentResult
    {
        private const string ContentTypeApplicationJson = "application/json";

        public SystemTextJsonResult(object value, JsonSerializerOptions? options = null)
        {
            ContentType = ContentTypeApplicationJson;
            Content = options == null ? JsonSerializer.Serialize(value) : JsonSerializer.Serialize(value, options);
        }
    }
}