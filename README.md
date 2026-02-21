# Flow.Launcher.Plugin.Shodan

Flow Launcher plugin to interact with the Shodan API --- the search
engine for Internet-connected devices.

------------------------------------------------------------------------

## 🚀 Installation

1.  Download the plugin\
2.  Place it in the Flow Launcher plugins folder\
3.  Restart Flow Launcher\
4.  Configure your Shodan API key in the plugin settings

------------------------------------------------------------------------

## 🔑 Configuration

Get your free API key at:\
https://account.shodan.io/

------------------------------------------------------------------------

## 📖 Available Commands

### `shodan host <ip>`

Displays detailed information about an IP address:

-   Organization\
-   Operating system\
-   Open ports\
-   Location (country, city)\
-   Detected vulnerabilities

**Example:**

    shodan host 8.8.8.8

------------------------------------------------------------------------

### `shodan search <query>`

Search for devices using Shodan filters:

-   Search by country: `country:FR`\
-   Search by product: `apache`\
-   Search by port: `port:80`

**Examples:**

    shodan search apache country:FR
    shodan search webcam port:8080
    shodan search nginx country:US city:Miami

------------------------------------------------------------------------

### `shodan dns <hostname>`

Resolves a domain name to its IP address

**Example:**

    shodan dns google.com

------------------------------------------------------------------------

### `shodan reverse <ip>`

Performs a reverse DNS lookup to find the domain name associated with an
IP

**Example:**

    shodan reverse 8.8.8.8

------------------------------------------------------------------------

### `shodan myip`

Displays your public IP address

------------------------------------------------------------------------

### `shodan info`

Shows information about your API account:

-   Current plan\
-   Remaining query credits\
-   Remaining scan credits

------------------------------------------------------------------------

### `shodan help`

Displays the list of all available commands

------------------------------------------------------------------------

## 💡 Tips

-   Click on a result to copy the IP address to your clipboard\
-   Searches are limited to 10 results for optimal performance\
-   Use Shodan filters to refine your searches

------------------------------------------------------------------------

## 🔍 Shodan Search Filters

Here are some useful filters:

-   `country:FR` -- Country (ISO code)\
-   `city:"Paris"` -- City\
-   `port:80` -- Specific port\
-   `os:"Windows"` -- Operating system\
-   `org:"Google"` -- Organization\
-   `product:"Apache"` -- Product/service\
-   `vuln:CVE-2014-0160` -- Specific vulnerability

------------------------------------------------------------------------

## 🔗 Useful Links

-   Shodan API Documentation: https://developer.shodan.io/\
-   Shodan Search Filters: https://www.shodan.io/search/filters\
-   Flow Launcher: https://www.flowlauncher.com/

------------------------------------------------------------------------

## 📌 Notes

-   Results are limited to 10 entries for performance reasons\
-   Requires a valid Shodan API key\
-   Internet connection required

------------------------------------------------------------------------

## 🛠 Requirements

-   Flow Launcher
-   .NET runtime (if required by your build)
-   Shodan API key
