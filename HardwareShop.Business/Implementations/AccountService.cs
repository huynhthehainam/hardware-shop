using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> accountRepository;
        private readonly IJwtService jwtService;
        public AccountService(IRepository<Account> accountRepository, IJwtService jwtService)
        {
            this.accountRepository = accountRepository;
            this.jwtService = jwtService;
        }

        public async Task<CreatedAccountDto> CreateAccountAsync(string accountName, string password)
        {
            return new CreatedAccountDto { Id = 1 };
        }

        public async Task<List<AccountDto>> GetAccountDtosAsync()
        {
            PageData<Account> accountPageData = await accountRepository.GetPageDataByQueryAsync(new PagingModel { PageIndex = 0, PageSize = 5 }, e => true, new List<QueryOrder<Account>>() { new QueryOrder<Account>(e => e.Username, true), new QueryOrder<Account>(e => e.HashedPassword, false) });
            return accountPageData.Items.Select(e => new AccountDto() { Id = e.Id }).ToList();
        }

        public async Task<LoginResponse?> Login(string username, string password)
        {
            var account = await accountRepository.GetItemByQueryAsync(e => e.Username == username);
            if (account == null) return null;
            var cacheAccount = new CacheAccount() { Id = account.Id, Username = account.Username ?? "", Role = account.Role };
            var response = jwtService.GenerateTokens(cacheAccount);
            return response;
        }
    }
}
