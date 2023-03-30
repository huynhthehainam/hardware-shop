




using System.Text.Json;
using HardwareShop.Business.Dtos;
using HardwareShop.Business.Helpers;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace HardwareShop.WebApi.Controllers
{
    public class SeedDataController : AuthorizedApiControllerBase
    {
        private class DbUnitModel
        {
            public string Name { get; set; }
            public List<string> Variants { get; set; }
            public int CategoryId { get; set; }
            public double StepNumber { get; set; }
            public double CompareWithPrimaryUnit { get; set; }
            public int? Id { get; set; }
            public DbUnitModel(string name, int categoryId, List<string> variants, double stepNumber, double compareWithPrimaryUnit)
            {
                Name = name;
                CategoryId = categoryId;
                Variants = variants;
                StepNumber = stepNumber;
                CompareWithPrimaryUnit = compareWithPrimaryUnit;
            }
        }
        private class DbDebtHistoryModel
        {
            public DateTime CreatedDate { get; set; }
            public double Amount { get; set; }
            public double OldDebt { get; set; }
            public double NewDebt { get; set; }
            public string Reason { get; set; }
            public DbDebtHistoryModel(DateTime createdDate, double amount, string reason)
            {
                CreatedDate = createdDate;
                Amount = amount;
                Reason = reason;
            }
            public Tuple<string, JsonDocument> GetReasonTuple()
            {

                switch (Reason)
                {
                    case "Mua hàng":
                        {
                            return CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying("");
                        }
                    case "Thêm nợ":
                        {
                            return CustomerDebtHistoryHelper.GenerateDebtReasonWhenBorrowing();
                        }
                    case "Trả tiền":
                        {
                            return CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingBack();
                        }
                    case "Xóa nợ":
                        {
                            return CustomerDebtHistoryHelper.GenerateDebtReasonWhenPayingAll();
                        }
                    default:
                        {
                            return CustomerDebtHistoryHelper.GenerateDebtReasonWhenBuying("");
                        }
                }

            }
        }

        private class DbCustomerModel
        {
            public int OldId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public double Debt { get; set; }
            public DbDebtHistoryModel[] DebtHistories { get; set; }
            public int? Id { get; set; }
            public DbCustomerModel(int oldId, string name, string address, double debt, IEnumerable<DbDebtHistoryModel> debtHistories)
            {
                OldId = oldId;
                Name = name;
                Address = address;
                Debt = debt;
                DebtHistories = debtHistories.ToArray();
            }
        }

        private class DbInvoiceModel
        {
            public double Deposit { get; set; }
            public DateTime CreatedDate { get; set; }
            public int? DebtHistoryId { get; set; }
            public double NewDebt { get; set; }
            public double OldDebt { get; set; }
            public int CustomerId { get; set; }
            public DbInvoiceModel(double deposit, DateTime createdDate, double newDebt, double oldDebt, int customerId)
            {
                Deposit = deposit;
                CreatedDate = createdDate;
                NewDebt = newDebt;
                OldDebt = oldDebt;
                CustomerId = customerId;
            }
        }
        private class DbInvoiceDetailModel
        {
            public int ProductId { get; set; }
            public double Quantity { get; set; }
            public int UnitId { get; set; }
            public double Price { get; set; }
            public double OriginalPrice { get; set; }
            public string? Description { get; set; }
            public DbInvoiceDetailModel(int productId, double quantity, int unitId, double price, double originalPrice, string? description)
            {
                ProductId = productId;
                Quantity = quantity;
                UnitId = unitId;
                Price = price;
                OriginalPrice = originalPrice;
                Description = description;
            }
        }
        private readonly IRepository<Unit> unitRepository;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<Shop> shopRepository;
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<Invoice> invoiceRepository;
        public SeedDataController(IRepository<Customer> customerRepository, IRepository<Shop> shopRepository, IRepository<Product> productRepository, IRepository<Unit> unitRepository, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService, IRepository<Invoice> invoiceRepository) : base(responseResultBuilder, currentUserService)
        {
            this.unitRepository = unitRepository;
            this.productRepository = productRepository;
            this.shopRepository = shopRepository;
            this.customerRepository = customerRepository;
            this.invoiceRepository = invoiceRepository;
        }
        [HttpPost("SeedFromDbFile")]
        public async Task<IActionResult> SeedFromDbFile([FromForm] SeedFromFileCommand command)
        {
            if (command.DbFile == null || command.ShopId == null)
            {
                return responseResultBuilder.Build();
            }
            string dbFilePath = Path.Combine("UploadedDb");
            _ = Directory.CreateDirectory(dbFilePath);
            dbFilePath = Path.Combine(dbFilePath, "dbab.db");
            using (FileStream fileStream = new(dbFilePath, FileMode.Create))
            {
                command.DbFile.CopyTo(fileStream);
            }
            using SqliteConnection connection = new($"Data source={dbFilePath}");
            connection.Open();
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            try
            {

                int singleCategoryId = 2;
                int lengthCategoryId = 4;
                int volumeCategoryId = 5;
                int massCategoryId = 1;
                List<DbUnitModel> dbUnits = new()
            {
                new DbUnitModel("Sheet", singleCategoryId, new List<string>{"Tâm", "Tấm", "tâm", "tấm"}, 1,1),
                new DbUnitModel("Sphere piece", singleCategoryId, new List<string>{"Viên", "vien", "viên", "viển"},1,1),
                new DbUnitModel("Piece", singleCategoryId,new List<string>{"cai", "cái"} ,1,1),
                new DbUnitModel("Bag", singleCategoryId,new List<string>{"bich", "bịch"} ,1,1),
                new DbUnitModel("Bar", singleCategoryId,new List<string>{"cay", "cây"},1,1),
                new DbUnitModel("Bottle", singleCategoryId, new List<string>{"chai"},1,1),
                new DbUnitModel("Roll", singleCategoryId, new List<string>{"cuon", "cuộn"},1,1),
                new DbUnitModel("Box", singleCategoryId, new List<string>{"hop"},1,1),
                new DbUnitModel("Meter", lengthCategoryId, new List<string>{"m", "mét"},0.01,1),
                new DbUnitModel("Ounce", massCategoryId, new List<string>{"lạng"},0.01,0.1),
                new DbUnitModel("Ball", singleCategoryId, new List<string>{"trái"},1,1),
                new DbUnitModel("Litter", volumeCategoryId, new List<string>{"lit"},0.01,1),
                new DbUnitModel("Can", singleCategoryId, new List<string>{"lon"} ,1,1),
                new DbUnitModel("Kg", massCategoryId,new List<string>{"kg"},0.01,1)
            };
                foreach (DbUnitModel dbUnit in dbUnits)
                {
                    CreateOrUpdateResponse<Unit> unit = await unitRepository.CreateOrUpdateAsync(new Unit()
                    {
                        CompareWithPrimaryUnit = dbUnit.CompareWithPrimaryUnit,
                        IsPrimary = false,
                        Name = dbUnit.Name,
                        StepNumber = dbUnit.StepNumber,
                        UnitCategoryId = dbUnit.CategoryId,
                    }, e => new { e.Name, e.UnitCategoryId }, e => new
                    {
                        e.Name
                    });
                    dbUnit.Id = unit.Entity.Id;
                }
                SqliteCommand getAllProductCommand = connection.CreateCommand();


                getAllProductCommand.CommandText = "SELECT * from Warehouses w";
                using SqliteDataReader getAllProductReader = getAllProductCommand.ExecuteReader();
                int index = 0;
                List<Product> products = new();
                while (getAllProductReader.Read())
                {
                    index++;
                    string name = getAllProductReader.GetString(1);
                    string unitName = getAllProductReader.GetString(3);
                    double pricePerMass = getAllProductReader.GetDouble(4);
                    double mass = getAllProductReader.GetDouble(5);
                    double percentForFamiliarCustomer = getAllProductReader.GetDouble(6);
                    double percentForCustomer = getAllProductReader.GetDouble(7);
                    double priceForFamiliarCustomer = getAllProductReader.GetDouble(8);
                    double priceForCustomer = getAllProductReader.GetDouble(9);
                    double originalPrice = getAllProductReader.GetDouble(11);
                    int? unitId = dbUnits.FirstOrDefault(e => e.Variants.Contains(unitName))?.Id;
                    if (unitId == null)
                    {
                        responseResultBuilder.AddInvalidFieldError($"Products[{index}].Unit");
                        continue;
                    }
                    CreateIfNotExistResponse<Product> createIfNotExistResponse = await productRepository.CreateIfNotExistsAsync(new Product()
                    {
                        Name = name,
                        HasAutoCalculatePermission = true,
                        Mass = mass,
                        OriginalPrice = originalPrice,
                        PercentForCustomer = percentForCustomer,
                        PercentForFamiliarCustomer = percentForFamiliarCustomer,
                        PriceForCustomer = priceForCustomer,
                        PriceForFamiliarCustomer = priceForFamiliarCustomer,
                        UnitId = unitId.Value,
                        ShopId = command.ShopId.Value,
                        PricePerMass = pricePerMass,

                    }, e => new { e.ShopId, e.Name });
                    products.Add(createIfNotExistResponse.Entity);

                }

                SqliteCommand getAllCustomerCommand = connection.CreateCommand();
                getAllCustomerCommand.CommandText = "SELECT * from Customers c ";

                using SqliteDataReader getAllCustomerReader = getAllCustomerCommand.ExecuteReader();
                List<DbCustomerModel> dbCustomers = new();

                while (getAllCustomerReader.Read())
                {
                    int oldId = getAllCustomerReader.GetInt32(0);
                    string name = getAllCustomerReader.GetString(1);
                    string address = getAllCustomerReader.GetString(2);
                    double currentDebt = getAllCustomerReader.GetDouble(3);
                    List<DbDebtHistoryModel> debtHistories = new();
                    SqliteCommand getAllCustomerDebtHistoryCommand = connection.CreateCommand();
                    getAllCustomerDebtHistoryCommand.CommandText = $"SELECT * from DeptHistories dh where dh.customer_id = {oldId} order by dh .created DESC ";
                    SqliteDataReader getAllCustomerDebtHistoryReader = getAllCustomerDebtHistoryCommand.ExecuteReader();
                    double debt = currentDebt;
                    while (getAllCustomerDebtHistoryReader.Read())
                    {
                        DateTime createdDate = getAllCustomerDebtHistoryReader.GetDateTime(1);
                        double amount = getAllCustomerDebtHistoryReader.GetDouble(2);
                        string reason = getAllCustomerDebtHistoryReader.GetString(3);
                        DbDebtHistoryModel debtHistory = new(createdDate, amount, reason)
                        {
                            NewDebt = debt,
                            OldDebt = debt - amount
                        };
                        debt = debtHistory.OldDebt;
                        debtHistories.Add(debtHistory);
                    }
                    dbCustomers.Add(new DbCustomerModel(oldId, name, address, currentDebt, debtHistories));

                }
                foreach (DbCustomerModel dbCustomer in dbCustomers)
                {


                    CreateIfNotExistResponse<Customer> createIfNotExistResponse = await customerRepository.CreateIfNotExistsAsync(new Customer
                    {
                        Address = dbCustomer.Address,
                        Name = dbCustomer.Name,
                        ShopId = command.ShopId.Value,
                        IsFamiliar = false,
                        Debt = new CustomerDebt
                        {
                            Amount = dbCustomer.Debt,
                            Histories = dbCustomer.DebtHistories.Select(e => new CustomerDebtHistory()
                            {
                                ChangeOfDebt = e.Amount,
                                CreatedDate = TimeZoneInfo.ConvertTimeToUtc(e.CreatedDate, timeZoneInfo),
                                NewDebt = e.NewDebt,
                                OldDebt = e.OldDebt,
                                Reason = e.GetReasonTuple().Item1,
                                ReasonParams = e.GetReasonTuple().Item2,
                            }).ToList()
                        }
                    }, e => new { e.ShopId, e.Name });
                    dbCustomer.Id = createIfNotExistResponse.Entity.Id;
                }
                SqliteCommand getAllInvoiceCommand = connection.CreateCommand();
                getAllInvoiceCommand.CommandText = "SELECT * from Invoices i ";

                SqliteDataReader getAllInvoiceReader = getAllInvoiceCommand.ExecuteReader();
                List<DbInvoiceModel> dbInvoices = new();
                while (getAllInvoiceReader.Read())
                {
                    int oldId = getAllInvoiceReader.GetInt32(0);
                    DateTime createdDate = getAllInvoiceReader.GetDateTime(1);
                    createdDate = TimeZoneInfo.ConvertTimeToUtc(createdDate, timeZoneInfo);
                    double newDebt = getAllInvoiceReader.GetDouble(2);
                    double deposit = getAllInvoiceReader.GetDouble(3);
                    double oldDebt = getAllInvoiceReader.GetDouble(4);
                    int oldCustomerId = getAllInvoiceReader.GetInt32(5);
                    SqliteCommand getAllInvoiceDetailCommand = connection.CreateCommand();
                    getAllInvoiceDetailCommand.CommandText = $"SELECT * from InvoiceDetails id where id.invoice_id = {oldId}";
                    SqliteDataReader getAllInvoiceDetailReader = getAllInvoiceDetailCommand.ExecuteReader();
                    bool isAllDetailValid = true;
                    List<DbInvoiceDetailModel> details = new();
                    int? customerId = dbCustomers.FirstOrDefault(e => e.OldId == oldCustomerId)?.Id;
                    while (getAllInvoiceDetailReader.Read())
                    {
                        string productName = getAllInvoiceDetailReader.GetString(1);
                        productName = productName.Trim();
                        double quantity = getAllInvoiceDetailReader.GetDouble(2);
                        string unitName = getAllInvoiceDetailReader.GetString(3);
                        double price = getAllInvoiceDetailReader.GetDouble(4);
                        double originalPrice = getAllInvoiceDetailReader.GetDouble(5);
                        string description = getAllInvoiceDetailReader.GetString(8);
                        int? productId = products.FirstOrDefault(e => e.Name == productName)?.Id;
                        int? unitId = dbUnits.FirstOrDefault(e => e.Variants.Contains(unitName))?.Id;
                        if (productId == null || unitId == null)
                        {
                            isAllDetailValid = false;
                            break;
                        }
                        details.Add(new DbInvoiceDetailModel(productId.Value, quantity, unitId.Value, price, originalPrice, description));
                    }

                    if (!isAllDetailValid || customerId == null)
                    {
                        Console.WriteLine("Invoice is invalid");
                        continue;
                    }
                    dbInvoices.Add(new DbInvoiceModel(deposit, createdDate, newDebt, oldDebt, customerId.Value));
                }
                foreach (DbInvoiceModel dbInvoice in dbInvoices)
                {
                    var createIfNotExistResponse = await invoiceRepository.CreateIfNotExistsAsync(new Invoice()
                    {
                        new Invoice(){
                            CreatedDate = dbInvoice.CreatedDate,
                        }
                    })
                }

            }

            catch (Exception)
            {

            }
            finally
            {
                connection.Close();
                SqliteConnection.ClearAllPools();
                System.IO.File.Delete(dbFilePath);
            }
            return responseResultBuilder.Build();
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
                    PercentForCustomer = command.PercentForCustomer,
                    PercentForFamiliarCustomer = command.PercentForFamiliarCustomer,
                    PriceForCustomer = command.PriceForCustomer ?? 0,
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