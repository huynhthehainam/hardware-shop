using HardwareShop.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IShopService
    {
        public Task<CreatedShopDto?> CreateShopAsync(string name, string? address);
    }
}
