using System.Text.Json;
using HardwareShop.Core.Constants;

namespace HardwareShop.Dal.Helpers
{
    public static class InterfaceSettingsHelper
    {
        private static JsonDocument? defaultSettings = null;
        public static JsonDocument GenerateDefaultInterfaceSettings()
        {
            if (defaultSettings == null)
            {
                var settings = new
                {
                    Layout = new
                    {
                        Style = "layout1",
                        Config = new
                        {
                            Scroll = "content",
                            Navbar = new
                            {
                                Display = true,
                                Folded = true,
                                Position = "left"
                            },
                            Toolbar = new
                            {
                                Display = true,
                                Style = "fixed",
                                Position = "below"
                            },
                            Footer = new
                            {
                                Display = false,
                                Style = "fixed",
                                Position = "below"
                            },
                            Mode = "fullwidth",
                        }
                    },
                    CustomScrollbars = true,
                    Theme = new
                    {
                        Main = "defaultDark",
                        Navbar = "defaultDark",
                        Toolbar = "defaultDark",
                        Footer = "defaultDark"
                    }
                };
                defaultSettings = JsonDocument.Parse(JsonSerializer.Serialize(settings, JsonSerializerConstants.CamelOptions));
            }
            return defaultSettings;
        }
    }
}
