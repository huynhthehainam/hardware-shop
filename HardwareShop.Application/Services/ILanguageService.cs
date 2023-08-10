using System.Text.Json.Serialization;

namespace HardwareShop.Application.Services
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupportedLanguage
    {
        Vietnamese,
        English
    }
    public interface ILanguageService
    {
        SupportedLanguage GetLanguage();
        string GenerateFullName(string firstName, string lastName);
        void SetLanguage(SupportedLanguage language);
        string Translate(string html, Dictionary<string, Dictionary<SupportedLanguage, string>> translation);
    }
}
