using System;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Shodan
{
    public partial class ShodanSettingsPanel : UserControl
    {
        private readonly ShodanSettings _settings;
        private readonly PluginInitContext _context;

        public ShodanSettingsPanel(ShodanSettings settings, PluginInitContext context)
        {
            _settings = settings;
            _context = context;
            InitializeComponent();
            ApiKeyTextBox.Text = _settings.ApiKey;
            ApplyTranslations();
        }

        private string T(string key, string fallback)
        {
            var translated = _context?.API?.GetTranslation(key);
            return string.IsNullOrWhiteSpace(translated) ? fallback : translated;
        }

        private void ApplyTranslations()
        {
            SettingsTitleTextBlock.Text = T("flowlauncher_plugin_shodan_settings_title", "Shodan API Settings");
            ApiKeyLabelTextBlock.Text = T("flowlauncher_plugin_shodan_settings_api_key", "API Key:");
            ApiKeyHintTextBlock.Text = T("flowlauncher_plugin_shodan_settings_api_key_hint", "Get your API key from: https://account.shodan.io/");
            SaveButton.Content = T("flowlauncher_plugin_shodan_settings_save", "Save");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.ApiKey = ApiKeyTextBox.Text;
            _context.API.SavePluginSettings();
            MessageBox.Show(
                T("flowlauncher_plugin_shodan_settings_saved", "Settings saved successfully!"),
                T("flowlauncher_plugin_shodan_plugin_name", "Shodan"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
