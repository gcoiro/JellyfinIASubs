using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JellyfinIASubs.Models;

namespace JellyfinIASubs.Services
{
    public sealed class AiSubtitleClient
    {
        private static readonly HttpClient HttpClient = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public async Task<AiSubtitleResponse> GenerateAsync(AiSubtitleRequest request, AiEndpointOptions options, CancellationToken cancellationToken)
        {
            var baseUrl = (options.BaseUrl ?? string.Empty).TrimEnd('/');
            var path = (options.GeneratePath ?? string.Empty).TrimStart('/');
            var url = string.IsNullOrWhiteSpace(path) ? baseUrl : $"{baseUrl}/{path}";

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("AI subtitle endpoint is not configured");
            }

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                var headerValue = options.UseBearerAuth ? $"Bearer {options.ApiKey}" : options.ApiKey;
                httpRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(headerValue);
            }

            var json = JsonSerializer.Serialize(request, JsonOptions);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (options.TimeoutSeconds > 0)
            {
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));
            }

            using var response = await HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token).ConfigureAwait(false);
            var parsed = JsonSerializer.Deserialize<AiSubtitleResponse>(body, JsonOptions);

            if (parsed == null || string.IsNullOrWhiteSpace(parsed.Srt))
            {
                throw new InvalidOperationException("AI subtitle response did not include SRT content");
            }

            return parsed;
        }
    }
}
