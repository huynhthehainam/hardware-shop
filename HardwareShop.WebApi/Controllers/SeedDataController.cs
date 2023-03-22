




using System.Text.Json;
using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public class SeedDataController : AuthorizedApiControllerBase
    {
        private readonly IRepository<Unit> unitRepository;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<Shop> shopRepository;
        private readonly IRepository<Customer> customerRepository;
        public SeedDataController(IRepository<Customer> customerRepository, IRepository<Shop> shopRepository, IRepository<Product> productRepository, IRepository<Unit> unitRepository, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.unitRepository = unitRepository;
            this.productRepository = productRepository;
            this.shopRepository = shopRepository;
            this.customerRepository = customerRepository;
        }
        [HttpPost("SeedUnits")]
        public async Task<IActionResult> SeedUnit([FromBody] List<SeedUnitCommand> commands)
        {
            List<UnitDto> unitDtos = new();
            foreach (SeedUnitCommand command in commands)
            {
                CreateIfNotExistResponse<Unit> createIfNotExistResponse = await unitRepository.CreateIfNotExistsAsync(new Unit()
                {
                    Name = command.Name ?? "",
                    StepNumber = command.Step ?? 0.0,
                    UnitCategoryId = command.CategoryId ?? 0,

                }, e => new { e.Name });
                Unit unit = createIfNotExistResponse.Entity;
                unitDtos.Add(new UnitDto() { Id = unit.Id, Name = unit.Name });
            }
            responseResultBuilder.SetData(unitDtos);
            return responseResultBuilder.Build();
        }
        [HttpPost("SeedProducts")]
        public async Task<IActionResult> SeedProducts([FromBody] List<SeedProductCommand> commands)
        {
            List<object> productDtos = new();
            foreach ((SeedProductCommand command, int index) in commands.Select((e, i) => (e, i)))
            {
                bool isShopExist = await shopRepository.AnyAsync(e => e.Id == command.ShopId);
                if (!isShopExist)
                {
                    responseResultBuilder.AddInvalidFieldError($"[{index}].ShopId");
                    return responseResultBuilder.Build();
                }
                bool isUnitExist = await unitRepository.AnyAsync(e => e.Id == command.UnitId);
                if (!isUnitExist)
                {
                    responseResultBuilder.AddInvalidFieldError($"[{index}].UnitId");
                    return responseResultBuilder.Build();
                }
                CreateIfNotExistResponse<Product> createIfNotExistResponse = await productRepository.CreateIfNotExistsAsync(new Product()
                {
                    Mass = command.Mass,
                    Name = command.Name ?? "",
                    OriginalPrice = command.OriginalPrice ?? 0,
                    HasAutoCalculatePermission = command.HasAutoCalculatePermission,
                    PercentForCustomer = command.PriceForCustomer,
                    PercentForFamiliarCustomer = command.PercentForFamiliarCustomer,
                    PriceForCustomer = command.PercentForCustomer ?? 0,
                    PriceForFamiliarCustomer = command.PriceForFamiliarCustomer,
                    PricePerMass = command.PricePerMass,

                    UnitId = command.UnitId ?? 0,
                    ShopId = command.ShopId ?? 0,
                }, e => new { e.Name, e.ShopId, e.UnitId });
                productDtos.Add(new { command.Uuid, createIfNotExistResponse.Entity.Id });
            }
            responseResultBuilder.SetData(productDtos);
            return responseResultBuilder.Build();
        }
        [HttpPost("SeedCustomers")]
        public async Task<IActionResult> SeedCustomers([FromBody] List<SeedCustomerCommand> commands)
        {
            List<object> dtos = new();

            foreach ((SeedCustomerCommand command, int index) in commands.Select((e, i) => (e, i)))
            {
                bool isShopExist = await shopRepository.AnyAsync(e => e.Id == command.ShopId);
                if (!isShopExist)
                {
                    responseResultBuilder.AddInvalidFieldError($"[{index}].ShopId");
                    return responseResultBuilder.Build();
                }
                List<SeedCustomerDebtHistoryCommand> histories = command.Histories;
                histories = histories.OrderByDescending(e => e.CreatedDate).ToList();
                double debt = command.Debt ?? 0;
                List<CustomerDebtHistory> finalHistories = new();
                foreach (SeedCustomerDebtHistoryCommand history in histories)
                {
                    string reason = history.Reason ?? "";
                    Tuple<string, JsonDocument> reasonTuple = new("", JsonDocument.Parse("{}"));
                    switch (reason)
                    {
                        case "Mua hàng":
                            {
                                reasonTuple = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying("");
                                break;
                            }
                        case "Thêm nợ":
                            {
                                reasonTuple = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBorrowing();
                                break;
                            }
                        case "Trả tiền":
                            {
                                reasonTuple = CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingBack();
                                break;
                            }
                        case "Xóa nợ":
                            {
                                reasonTuple = CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingAll();
                                break;
                            }
                        default:
                            {
                                reasonTuple = CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying("");
                                break;
                            }

                    }
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(history.TimeZone);
                    DateTime createdDate = TimeZoneInfo.ConvertTimeToUtc(history.CreatedDate ?? new DateTime(), timeZoneInfo);
                    double amount = history.Amount ?? 0;
                    CustomerDebtHistory finalHistory = new()
                    {
                        ChangeOfDebt = amount,
                        NewDebt = debt,
                        OldDebt = debt - amount,
                        CreatedDate = createdDate,
                        Reason = reasonTuple.Item1,
                        ReasonParams = reasonTuple.Item2,
                    };
                    finalHistories.Add(finalHistory);
                    debt = finalHistory.OldDebt;
                }
                CreateIfNotExistResponse<Customer> createIfNotExistResponse = await customerRepository.CreateIfNotExistsAsync(new Customer()
                {
                    Address = command.Address,
                    Name = command.Name,
                    ShopId = command.ShopId ?? 0,
                    Debt = new CustomerDebt()
                    {
                        Amount = command.Debt ?? 0,
                        Histories = finalHistories,
                    }

                }, e => new { e.Name, e.ShopId });
                dtos.Add(new { command.Uuid, createIfNotExistResponse.Entity.Id });
            }
            responseResultBuilder.SetData(dtos);
            return responseResultBuilder.Build();
        }
    }
}