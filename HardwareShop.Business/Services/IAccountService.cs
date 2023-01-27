using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IAccountService
    {
        Task<CreatedAccountDto> CreateAccountAsync(string accountName, string password);
        Task<List<AccountDto>> GetAccountDtosAsync();
        Task<LoginResponse?> Login(string username, string password);

    }
}
