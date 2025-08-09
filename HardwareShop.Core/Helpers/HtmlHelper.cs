


using System.Text;
using System.Text.RegularExpressions;

namespace HardwareShop.Core.Helpers
{
    public static class HtmlHelper
    {
        public static string ReplaceKeyWithValue(string html, Dictionary<string, string> values)
        {
            StringBuilder sb = new(html);
            foreach (string key in values.Keys)
            {
                string keyPattern = @"(?<key>{" + key + "})";
                MatchCollection matches = Regex.Matches(html, keyPattern);
                foreach (Match match in matches.Cast<Match>())
                {
                    string value = match.Groups["key"].Value;
                    _ = sb.Replace(value, values[key]);
                }
            }
            return sb.ToString();
        }
    }
}