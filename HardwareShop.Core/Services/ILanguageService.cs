using HardwareShop.Core.Implementations;

namespace HardwareShop.Core.Services
{

    public interface ILanguageService
    {
        SupportedLanguage GetLanguage();
        string GenerateFullName(string firstName, string lastName);
        void SetLanguage(SupportedLanguage language);
    }
}
