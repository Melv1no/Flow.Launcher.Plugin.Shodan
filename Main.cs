using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.Shodan.Services;
using Flow.Launcher.Plugin.Shodan.Helpers;

namespace Flow.Launcher.Plugin.Shodan
{
    public class Shodan : IAsyncPlugin, ISettingProvider, IPluginI18n
    {
        private PluginInitContext _context;
        private ShodanSettings _settings;
        private ShodanApiService _apiService;
        private string _currentApiKey;

        public Task InitAsync(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<ShodanSettings>();
            return Task.CompletedTask;
        }

        public async Task<List<Result>> QueryAsync(Query query, System.Threading.CancellationToken token)
        {
            var searchText = query.Search.Trim();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetHelpResults();
            }

            var parts = searchText.Split(new[] { ' ' }, 2);
            var command = parts[0].ToLower();
            if (CommandRequiresApiKey(command) && string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                return new List<Result> { ResultHelper.CreateConfigurationNeededResult(_context) };
            }

            EnsureApiService();

            try
            {
                return command switch
                {
                    "host" when parts.Length > 1 => await HandleHostLookupAsync(parts[1]),
                    "search" when parts.Length > 1 => await HandleSearchAsync(parts[1]),
                    "info" => await HandleApiInfoAsync(),
                    "myip" => await HandleMyIpAsync(),
                    "dns" when parts.Length > 1 => await HandleDnsResolveAsync(parts[1]),
                    "reverse" when parts.Length > 1 => await HandleDnsReverseAsync(parts[1]),
                    "help" => GetHelpResults(),
                    _ => new List<Result>
                    {
                        ResultHelper.CreateErrorResult(
                            T("flowlauncher_plugin_shodan_unknown_command_title", "Unknown command"),
                            T("flowlauncher_plugin_shodan_unknown_command_subtitle", "Type 'help' to see all available commands")
                        )
                    }
                };
            }
            catch (Exception ex)
            {
                return new List<Result>
                {
                    ResultHelper.CreateErrorResult(T("flowlauncher_plugin_shodan_error_title", "Error"), ex.Message)
                };
            }
        }

        private List<Result> GetHelpResults()
        {
            return new List<Result>
            {
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_host_title", "host <ip> - Information about an IP"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_host_subtitle", "Ex: host 8.8.8.8"),
                    IcoPath = "img/icon.png"
                },
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_search_title", "search <query> - Search devices"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_search_subtitle", "Ex: search apache country:FR"),
                    IcoPath = "img/icon.png"
                },
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_dns_title", "dns <hostname> - Resolve a domain"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_dns_subtitle", "Ex: dns google.com"),
                    IcoPath = "img/icon.png"
                },
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_reverse_title", "reverse <ip> - Reverse DNS"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_reverse_subtitle", "Ex: reverse 8.8.8.8"),
                    IcoPath = "img/icon.png"
                },
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_myip_title", "myip - Your public IP"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_myip_subtitle", "Get your public IP address"),
                    IcoPath = "img/icon.png"
                },
                new Result
                {
                    Title = T("flowlauncher_plugin_shodan_help_info_title", "info - Your account info"),
                    SubTitle = T("flowlauncher_plugin_shodan_help_info_subtitle", "Show your Shodan credits"),
                    IcoPath = "img/icon.png"
                }
            };
        }

        private async Task<List<Result>> HandleHostLookupAsync(string ip)
        {
            var hostInfo = await _apiService.GetHostInfoAsync(ip);
            return ResultHelper.CreateHostResults(hostInfo, _context);
        }

        private async Task<List<Result>> HandleSearchAsync(string searchQuery)
        {
            var searchResult = await _apiService.SearchAsync(searchQuery);

            var results = new List<Result>
            {
                new Result
                {
                    Title = string.Format(T("flowlauncher_plugin_shodan_search_found_title", "{0:N0} results found"), searchResult.Total),
                    SubTitle = string.Format(T("flowlauncher_plugin_shodan_search_found_subtitle", "Showing first 10 for: {0}"), searchQuery),
                    IcoPath = "img/icon.png",
                    Action = c =>
                    {
                        _context.API.OpenUrl($"https://www.shodan.io/search?query={Uri.EscapeDataString(searchQuery)}");
                        return true;
                    }
                }
            };

            results.AddRange(
                searchResult.Matches
                    .Take(10)
                    .Select(match => ResultHelper.CreateSearchResult(match, _context))
            );

            return results;
        }

        private async Task<List<Result>> HandleApiInfoAsync()
        {
            var apiInfo = await _apiService.GetApiInfoAsync();

            return new List<Result>
            {
                new Result
                {
                    Title = string.Format(T("flowlauncher_plugin_shodan_info_title", "Plan: {0}"), apiInfo.Plan),
                    SubTitle = string.Format(T("flowlauncher_plugin_shodan_info_subtitle", "Search credits: {0} | Scan credits: {1}"), apiInfo.QueryCredits, apiInfo.ScanCredits),
                    IcoPath = "img/icon.png",
                    Action = c =>
                    {
                        _context.API.CopyToClipboard(string.Format(
                            T("flowlauncher_plugin_shodan_info_clipboard", "Plan: {0}, Query: {1}, Scan: {2}"),
                            apiInfo.Plan,
                            apiInfo.QueryCredits,
                            apiInfo.ScanCredits));
                        return true;
                    }
                }
            };
        }

        private async Task<List<Result>> HandleMyIpAsync()
        {
            var ip = await _apiService.GetMyIpAsync();

            return new List<Result>
            {
                new Result
                {
                    Title = string.Format(T("flowlauncher_plugin_shodan_myip_title", "Your IP: {0}"), ip),
                    SubTitle = T("flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = c =>
                    {
                        _context.API.OpenUrl($"https://www.shodan.io/host/{Uri.EscapeDataString(ip)}");
                        return true;
                    }
                }
            };
        }

        private async Task<List<Result>> HandleDnsResolveAsync(string hostname)
        {
            var ip = await _apiService.GetDnsResolveAsync(hostname);

            return new List<Result>
            {
                new Result
                {
                    Title = $"{hostname} -> {ip}",
                    SubTitle = T("flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = c =>
                    {
                        _context.API.OpenUrl($"https://www.shodan.io/host/{Uri.EscapeDataString(ip)}");
                        return true;
                    }
                }
            };
        }

        private async Task<List<Result>> HandleDnsReverseAsync(string ip)
        {
            var hostname = await _apiService.GetDnsReverseAsync(ip);

            return new List<Result>
            {
                new Result
                {
                    Title = $"{ip} -> {hostname}",
                    SubTitle = T("flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = c =>
                    {
                        _context.API.OpenUrl($"https://www.shodan.io/host/{Uri.EscapeDataString(ip)}");
                        return true;
                    }
                }
            };
        }

        public System.Windows.Controls.Control CreateSettingPanel()
        {
            return new ShodanSettingsPanel(_settings, _context);
        }

        public string GetTranslatedPluginTitle()
        {
            return T("flowlauncher_plugin_shodan_plugin_name", "Shodan");
        }

        public string GetTranslatedPluginDescription()
        {
            return T("flowlauncher_plugin_shodan_plugin_description", "Interact with the Shodan API");
        }

        public void OnCultureInfoChanged(CultureInfo cultureInfo)
        {
        }

        private string T(string key, string fallback)
        {
            var translated = _context?.API?.GetTranslation(key);
            return string.IsNullOrWhiteSpace(translated) ? fallback : translated;
        }

        private bool CommandRequiresApiKey(string command)
        {
            return command is "search" or "dns" or "reverse" or "info";
        }

        private void EnsureApiService()
        {
            var apiKey = _settings?.ApiKey ?? string.Empty;
            if (_apiService == null || !string.Equals(_currentApiKey, apiKey, StringComparison.Ordinal))
            {
                _apiService = new ShodanApiService(apiKey);
                _currentApiKey = apiKey;
            }
        }
    }
}
