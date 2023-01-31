using HardwareShop.Core.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupportedLanguage
    {
        Vietnamese,
        English
    }
    public class LanguageConfiguration
    {
        public SupportedLanguage Language { get; set; } = SupportedLanguage.English;
    }
    public class LanguageService : ILanguageService
    {
        private readonly LanguageConfiguration configuration;
        public LanguageConfiguration GetConfiguration()
        {
            return configuration;
        }

        public string GenerateFullName(string firstName, string lastName)
        {
            var language = configuration.Language;
            switch (language)
            {
                case SupportedLanguage.English:
                    return $"{firstName} {lastName}";
                case SupportedLanguage.Vietnamese:
                    return $"{lastName} {firstName}";
                default:
                    return $"{firstName} {lastName}";
            }
        }
        public LanguageService(IOptions<LanguageConfiguration> options)
        {
            configuration = options.Value;
        }
    }
}
