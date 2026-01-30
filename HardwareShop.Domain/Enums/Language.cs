
namespace HardwareShop.Domain.Enums;

public enum Language
{
    English = 1,
    Vietnamese = 2
}

public static class LanguageExtensions
{
    public static string ToLanguageCode(this Language language)
    {
        return language switch
        {
            Language.English => "en",
            Language.Vietnamese => "vi",
            _ => "en"
        };
    }
}
