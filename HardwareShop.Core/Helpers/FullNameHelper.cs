using HardwareShop.Core.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Helpers
{
    public static class FullNameHelper
    {
        public static string GetFullName(ResponseResultLanguage language, string firstName, string lastName)
        {
            switch (language)
            {
                case ResponseResultLanguage.English:
                    return $"{firstName} {lastName}";
                case ResponseResultLanguage.Vietnamese:
                    return $"{lastName} {firstName}";
                default:
                    return $"{firstName} {lastName}";
            }
        }
    }
}
