


using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Customer> customerRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        public CustomerService(IResponseResultBuilder responseResultBuilder, IShopService shopService, IRepository<Customer> customerRepository)
        {
            this.responseResultBuilder = responseResultBuilder;
            this.customerRepository = customerRepository;
            this.shopService = shopService;

        }
        public async Task<PageData<CustomerDto>?> GetCustomerPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customers = await customerRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id, string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            }), new List<QueryOrder<Customer>> { new QueryOrder<Customer>(e => e.Name, true) });
            return PageData<CustomerDto>.ConvertFromOtherPageData(customers, e => new CustomerDto
            {
                Id = e.Id
            });
        }
        public async Task<PageData<CustomerDto>?> GetCustomerInDebtPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customers = await customerRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id && (e.Debt == null || e.Debt.Amount > 0), string.IsNullOrEmpty(search) ? null : new SearchQuery<Customer>(search, e => new
            {
                e.Name,
                e.Address,
                e.Phone
            }), new List<QueryOrder<Customer>> { new QueryOrder<Customer>(e => e.Name, true) });
            return PageData<CustomerDto>.ConvertFromOtherPageData(customers, e => new CustomerDto
            {
                Id = e.Id
            });
        }

        public async Task<CreatedCustomerDto?> CreateCustomerOfCurrentUserShopAsync(string name, string? phone, string? address, bool isFamiliar)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }

            var customer = await customerRepository.CreateIfNotExistsAsync(new Customer
            {
                ShopId = shop.Id,
                Name = name,
                Phone = phone,
                Address = address,
                IsFamiliar = isFamiliar
            }, e => new { e.Name, e.Address, e.Phone });
            if (customer == null)
            {
                responseResultBuilder.AddExistedEntityError("Customer");
                return null;
            }
            return new CreatedCustomerDto { Id = customer.Id };

        }

        public async Task<CustomerDto?> UpdateCustomerOfCurrentUserShopAsync(int customerId, string? name, string? phone, string? address, bool? isFamiliar, double? amountOfCash)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var customer = await customerRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == customerId);
            if (customer == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Customer");
                return null;
            }
            customer.Name = string.IsNullOrEmpty(name) ? customer.Name : name;
            customer.Phone = string.IsNullOrEmpty(phone) ? customer.Phone : phone;
            customer.Address = string.IsNullOrEmpty(address) ? customer.Address : address;
            customer.IsFamiliar = isFamiliar == null ? customer.IsFamiliar : isFamiliar.Value;
            if (amountOfCash != null)
            {
                if (customer.Debt == null)
                {
                    customer.Debt = new CustomerDebt { Amount = amountOfCash.Value };
                }
                else
                {

                    if (customer.Debt.Histories != null)
                    {
                        var reason = CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingBack();
                        customer.Debt.Histories.Add(new CustomerDebtHistory
                        {
                            ChangeOfDebt = amountOfCash.Value,
                            CreatedDate = DateTime.UtcNow,
                            OldDebt = customer.Debt.Amount,
                            NewDebt = customer.Debt.Amount + amountOfCash.Value,
                            Reason = reason.Item1,
                            ReasonParams = reason.Item2,
                        });
                    }
                    customer.Debt.Amount += amountOfCash.Value;
                }
            }
            customer = await customerRepository.UpdateAsync(customer);

            return new CustomerDto { Id = customer.Id };
        }
    }
}