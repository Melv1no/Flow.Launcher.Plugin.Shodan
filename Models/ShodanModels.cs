using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Shodan
{
    public class ShodanHostInfo
    {
        [JsonPropertyName("ip_str")]
        public string IpAddress { get; set; }

        [JsonPropertyName("org")]
        public string Organization { get; set; }

        [JsonPropertyName("os")]
        public string OperatingSystem { get; set; }

        [JsonPropertyName("ports")]
        public List<int> Ports { get; set; }

        [JsonPropertyName("hostnames")]
        public List<string> Hostnames { get; set; }

        [JsonPropertyName("country_name")]
        public string Country { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("isp")]
        public string Isp { get; set; }

        [JsonPropertyName("asn")]
        public string Asn { get; set; }

        [JsonPropertyName("last_update")]
        public string LastUpdate { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("vulns")]
        public JsonElement Vulnerabilities { get; set; }
    }

    public class ShodanSearchResult
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("matches")]
        public List<ShodanMatch> Matches { get; set; }
    }

    public class ShodanMatch
    {
        [JsonPropertyName("ip_str")]
        public string IpAddress { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("org")]
        public string Organization { get; set; }

        [JsonPropertyName("hostnames")]
        public List<string> Hostnames { get; set; }

        [JsonPropertyName("location")]
        public ShodanLocation Location { get; set; }

        [JsonPropertyName("product")]
        public string Product { get; set; }
    }

    public class ShodanLocation
    {
        [JsonPropertyName("country_name")]
        public string Country { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
    }

    public class ShodanApiInfo
    {
        [JsonPropertyName("query_credits")]
        public int QueryCredits { get; set; }

        [JsonPropertyName("scan_credits")]
        public int ScanCredits { get; set; }

        [JsonPropertyName("plan")]
        public string Plan { get; set; }
    }
}
