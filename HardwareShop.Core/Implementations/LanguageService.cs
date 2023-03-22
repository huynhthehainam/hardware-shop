using System.Text.Json.Serialization;
using HardwareShop.Core.Helpers;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Core.Implementations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupportedLanguage
    {
        Vietnamese,
        English
    }
    public class LanguageService : ILanguageService
    {
        private SupportedLanguage supportedLanguage;
        public string GenerateFullName(string firstName, string lastName)
        {
            var language = supportedLanguage;
            return language switch
            {
                SupportedLanguage.English => $"{firstName} {lastName}",
                SupportedLanguage.Vietnamese => $"{lastName} {firstName}",
                _ => $"{firstName} {lastName}",
            };
        }

        public void SetLanguage(SupportedLanguage language)
        {
            supportedLanguage = language;
        }

        public SupportedLanguage GetLanguage()
        {
            return supportedLanguage;
        }

        public string Translate(string html, Dictionary<string, Dictionary<SupportedLanguage, string>> translations)
        {
            Dictionary<string, string> values = new();
            foreach (var key in translations.Keys)
            {
                var translationKey = $"TRANSLATE_{key}";
                var translation = translations[key][supportedLanguage];
                values.Add(translationKey, translation);
            }

            return HtmlHelper.ReplaceKeyWithValue(html, values);
        }

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            var language = httpContextAccessor.HttpContext?.Request.Query["lang"].ToString();

            var success = Enum.TryParse<SupportedLanguage>(language, true, out SupportedLanguage supportedLanguage);
            if (success)
            {
                this.supportedLanguage = supportedLanguage;
            }
            else
            {
                this.supportedLanguage = SupportedLanguage.English;
            }
        }
    }
}
