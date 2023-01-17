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
        private readonly IRepository<Account, int> accountRepository;
        public AccountService(IRepository<Account, int> accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<CreatedAccountDto> CreateAccountAsync(string accountName, string password)
        {
            return new CreatedAccountDto { Id = 1 };
        }

        public async Task<List<AccountDto>> GetAccountDtosAsync()
        {
            PageData<Account, int> accountPageData = await accountRepository.GetPageDataByQueryAsync(new PagingModel { PageIndex = 0, PageSize = 5 }, e => true, new List<QueryOrder<Account, int>>() { new QueryOrder<Account, int>(e => e.Username, true), new QueryOrder<Account, int>(e => e.HashedPassword, false) });
            return accountPageData.Items.Select(e => new AccountDto() { Id = e.Id }).ToList();
        }
    }
}
