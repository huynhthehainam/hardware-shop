using HardwareShop.Core.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Core.Services
{
    
    public interface ILanguageService
    {
        LanguageConfiguration GetConfiguration();
        string GenerateFullName(string firstName, string lastName);
    }
}
