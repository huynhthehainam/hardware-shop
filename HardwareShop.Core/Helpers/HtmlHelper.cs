


using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HardwareShop.Core.Helpers
{
    public static class HtmlHelper
    {
        public static string ReplaceKeyWithValue(string html, Dictionary<string, string> values)
        {
            StringBuilder sb = new StringBuilder(html);
            foreach (var key in values.Keys)
            {
                var keyPattern = @"(?<key>{" + key + "})";
                var matches = Regex.Matches(html, keyPattern);
                foreach (Match match in matches)
                {
                    var value = match.Groups["key"].Value;
                    sb.Replace(value, values[key]);
                }
            }
            return sb.ToString();
        }
    }
}