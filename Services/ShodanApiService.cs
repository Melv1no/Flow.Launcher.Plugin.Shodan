using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Flow.Launcher.Plugin.Shodan.Services
{
    public class ShodanApiService
    {
        private readonly string _apiKey;
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://api.shodan.io";
        private readonly JsonSerializerOptions _jsonOptions;

        public ShodanApiService(string apiKey)
        {
            _apiKey = apiKey;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ShodanHostInfo> GetHostInfoAsync(string ip)
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/shodan/host/{ip}");
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<ShodanHostInfo>(response, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Host lookup failed: {ex.Message}");
            }
        }

        public async Task<ShodanSearchResult> SearchAsync(string query, int page = 1)
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/shodan/host/search?query={Uri.EscapeDataString(query)}&page={page}");
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<ShodanSearchResult>(response, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Search failed: {ex.Message}");
            }
        }

        public async Task<ShodanApiInfo> GetApiInfoAsync()
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/api-info");
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<ShodanApiInfo>(response, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API info request failed: {ex.Message}");
            }
        }

        public async Task<string> GetMyIpAsync()
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/tools/myip");
                var response = await _httpClient.GetStringAsync(url);
                return response.Trim('"');
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"My IP request failed: {ex.Message}");
            }
        }

        public async Task<string> GetDnsResolveAsync(string hostname)
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/dns/resolve?hostnames={Uri.EscapeDataString(hostname)}");
                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(response, _jsonOptions);
                return result != null && result.ContainsKey(hostname) ? result[hostname] : "Not found";
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"DNS resolve request failed: {ex.Message}");
            }
        }

        public async Task<string> GetDnsReverseAsync(string ip)
        {
            try
            {
                var url = WithApiKey($"{BaseUrl}/dns/reverse?ips={Uri.EscapeDataString(ip)}");
                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(response, _jsonOptions);
                
                if (result != null && result.ContainsKey(ip))
                {
                    var value = result[ip];
                    if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        var hostnames = JsonSerializer.Deserialize<List<string>>(jsonElement.GetRawText());
                        return hostnames?.Count > 0 ? hostnames[0] : "Not found";
                    }
                }
                return "Not found";
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Reverse DNS request failed: {ex.Message}");
            }
        }

        private string WithApiKey(string url)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return url;
            }

            var separator = url.Contains("?") ? "&" : "?";
            return $"{url}{separator}key={Uri.EscapeDataString(_apiKey)}";
        }
    }
}
