using System.ComponentModel;

namespace Flow.Launcher.Plugin.Shodan
{
    public class ShodanSettings
    {
        [DisplayName("Shodan API Key")]
        [Description("Your Shodan API key (get it from https://account.shodan.io/)")]
        public string ApiKey { get; set; } = string.Empty;
    }
}
