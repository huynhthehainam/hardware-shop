﻿using System.Text.Json.Serialization;
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

        public void SetLanguage(SupportedLanguage language)
        {
            supportedLanguage = language;
        }

        public SupportedLanguage GetLanguage()
        {
            return supportedLanguage;
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
