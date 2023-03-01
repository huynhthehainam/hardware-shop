using System.Text.Json;
using HardwareShop.Core.Constants;

namespace HardwareShop.Business.Helpers
{
    public static class InterfaceSettingsHelper
    {
        private static JsonDocument defaultSettings = JsonDocument.Parse(JsonSerializer.Serialize(new
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
                        Display = true,
                        Style = "fixed",
                        Position = "below"
                    },
                    Mode = "fullwidth"
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
        }, JsonSerializerConstants.CamelOptions));
        public static JsonDocument GenerateDefaultInterfaceSettings()
        {
            return defaultSettings;
        }
    }
}
