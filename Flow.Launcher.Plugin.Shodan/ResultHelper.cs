using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Shodan.Helpers
{
    public static class ResultHelper
    {
        private static string T(PluginInitContext context, string key, string fallback)
        {
            var translated = context?.API?.GetTranslation(key);
            return string.IsNullOrWhiteSpace(translated) ? fallback : translated;
        }

        public static Result CreateErrorResult(string title, string subtitle, string iconPath = "img/icon.png")
        {
            return new Result
            {
                Title = title,
                SubTitle = subtitle,
                IcoPath = iconPath
            };
        }

        public static Result CreateConfigurationNeededResult(PluginInitContext context)
        {
            return new Result
            {
                Title = T(context, "flowlauncher_plugin_shodan_config_missing_title", "Shodan API key not configured"),
                SubTitle = T(context, "flowlauncher_plugin_shodan_config_missing_subtitle", "Please configure your API key in plugin settings"),
                IcoPath = "img/icon.png",
                Action = c =>
                {
                    context.API.OpenSettingDialog();
                    return true;
                }
            };
        }

        public static List<Result> CreateHostResults(ShodanHostInfo host, PluginInitContext context)
        {
            var results = new List<Result>();
            var hostUrl = $"https://www.shodan.io/host/{host.IpAddress}";

            var portsStr = host.Ports != null && host.Ports.Any()
                ? string.Join(", ", host.Ports.Take(20))
                : T(context, "flowlauncher_plugin_shodan_none", "None");

            results.Add(new Result
            {
                Title = $"{host.IpAddress} - {host.Organization ?? T(context, "flowlauncher_plugin_shodan_org_unknown", "Unknown organization")}",
                SubTitle = string.Format(
                    T(context, "flowlauncher_plugin_shodan_host_subtitle", "Location: {0}, {1} | OS: {2} | Ports: {3}"),
                    host.Country,
                    host.City,
                    host.OperatingSystem ?? T(context, "flowlauncher_plugin_shodan_unknown", "Unknown"),
                    portsStr),
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, hostUrl)
            });

            results.Add(new Result
            {
                Title = $"{T(context, "flowlauncher_plugin_shodan_field_org", "Organization")}: {host.Organization ?? T(context, "flowlauncher_plugin_shodan_unknown", "Unknown")}",
                SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, hostUrl)
            });

            results.Add(new Result
            {
                Title = $"{T(context, "flowlauncher_plugin_shodan_field_isp", "ISP")}: {host.Isp ?? T(context, "flowlauncher_plugin_shodan_unknown", "Unknown")}",
                SubTitle = $"{T(context, "flowlauncher_plugin_shodan_field_asn", "ASN")}: {host.Asn ?? T(context, "flowlauncher_plugin_shodan_unknown", "Unknown")}",
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, hostUrl)
            });

            results.Add(new Result
            {
                Title = $"{T(context, "flowlauncher_plugin_shodan_field_location", "Location")}: {host.Country}, {host.City}",
                SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, hostUrl)
            });

            results.Add(new Result
            {
                Title = $"{T(context, "flowlauncher_plugin_shodan_field_ports", "Open ports")}: {portsStr}",
                SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, hostUrl)
            });

            var hostnames = host.Hostnames?.Where(h => !string.IsNullOrWhiteSpace(h)).Take(5).ToList() ?? new List<string>();
            if (hostnames.Any())
            {
                foreach (var hostname in hostnames)
                {
                    results.Add(new Result
                    {
                        Title = $"{T(context, "flowlauncher_plugin_shodan_field_hostname", "Hostname")}: {hostname}",
                        SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                        IcoPath = "img/icon.png",
                        Action = OpenUrlAction(context, hostUrl)
                    });
                }
            }

            var vulnerabilities = ExtractVulnerabilities(host.Vulnerabilities).Take(8).ToList();
            if (vulnerabilities.Any())
            {
                results.Add(new Result
                {
                    Title = $"{T(context, "flowlauncher_plugin_shodan_field_vulns", "Vulnerabilities")}: {string.Join(", ", vulnerabilities)}",
                    SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = OpenUrlAction(context, hostUrl)
                });
            }

            if (host.Tags != null && host.Tags.Any())
            {
                results.Add(new Result
                {
                    Title = $"{T(context, "flowlauncher_plugin_shodan_field_tags", "Tags")}: {string.Join(", ", host.Tags.Take(8))}",
                    SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = OpenUrlAction(context, hostUrl)
                });
            }

            if (!string.IsNullOrWhiteSpace(host.LastUpdate))
            {
                results.Add(new Result
                {
                    Title = $"{T(context, "flowlauncher_plugin_shodan_field_last_update", "Last update")}: {host.LastUpdate}",
                    SubTitle = T(context, "flowlauncher_plugin_shodan_press_enter_open_host", "Press Enter to open on Shodan"),
                    IcoPath = "img/icon.png",
                    Action = OpenUrlAction(context, hostUrl)
                });
            }

            return results;
        }

        public static Result CreateSearchResult(ShodanMatch match, PluginInitContext context)
        {
            var location = match.Location != null 
                ? $"{match.Location.Country}, {match.Location.City}" 
                : T(context, "flowlauncher_plugin_shodan_location_unknown", "Unknown location");
            
            var product = !string.IsNullOrEmpty(match.Product) ? $" | {match.Product}" : "";

            return new Result
            {
                Title = $"{match.IpAddress}:{match.Port}",
                SubTitle = $"{match.Organization ?? T(context, "flowlauncher_plugin_shodan_unknown", "Unknown")} | {location}{product}",
                IcoPath = "img/icon.png",
                Action = OpenUrlAction(context, $"https://www.shodan.io/host/{match.IpAddress}")
            };
        }

        private static Func<ActionContext, bool> OpenUrlAction(PluginInitContext context, string url)
        {
            return c =>
            {
                context.API.OpenUrl(url);
                return true;
            };
        }

        private static IEnumerable<string> ExtractVulnerabilities(JsonElement vulnerabilities)
        {
            if (vulnerabilities.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in vulnerabilities.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        yield return item.GetString();
                    }
                }
                yield break;
            }

            if (vulnerabilities.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in vulnerabilities.EnumerateObject())
                {
                    yield return property.Name;
                }
            }
        }
    }
}
