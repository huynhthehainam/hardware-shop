




using System.Text.Json;
using HardwareShop.Business.Helpers;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.WebApi.Controllers
{
    public class SeedDataController : AuthorizedApiControllerBase
    {
        #region SqliteModels
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
            public string Code { get; set; }
            public DbInvoiceDetailModel[] Details { get; set; }
            public DbInvoiceModel(double deposit, DateTime createdDate, double newDebt, double oldDebt, int customerId, string code, IEnumerable<DbInvoiceDetailModel> details)
            {
                Deposit = deposit;
                CreatedDate = createdDate;
                NewDebt = newDebt;
                OldDebt = oldDebt;
                CustomerId = customerId;
                Code = code;
                Details = details.ToArray();
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
        #endregion

        private readonly IWebHostEnvironment environment;
        private readonly DbContext db;
        public SeedDataController(IWebHostEnvironment environment, DbContext db, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.db = db;
            this.environment = environment;
        }
        [HttpPost("SeedFromDbFile")]
        public Task<IActionResult> SeedFromDbFile([FromForm] SeedFromFileCommand command)
        {
            if (command.DbFile == null || command.ShopId == null)
            {
                return Task.FromResult(responseResultBuilder.Build());
            }
            var productAsset = db.Set<Asset>().FirstOrDefault(e => e.Filename == "ProductAsset.jpg");
            if (productAsset == null)
            {
                return Task.FromResult(responseResultBuilder.Build());
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
                #region SeedUnits
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
                    CreateOrUpdateResponse<Unit> unit = db.CreateOrUpdate(new Unit()
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
                #endregion

                #region SeedProducts
                SqliteCommand getAllProductCommand = connection.CreateCommand();
                getAllProductCommand.CommandText = "SELECT * from Warehouses w";
                using SqliteDataReader getAllProductReader = getAllProductCommand.ExecuteReader();
                int index = 0;
                List<Product> products = new();
                const string assetFolder = "SampleImages";
                const string productAssetFile = "ProductAsset.jpg";
                string productAssetPath = Path.Join(assetFolder, productAssetFile);
                byte[] productAssetBytes = System.IO.File.ReadAllBytes(productAssetPath);
                while (getAllProductReader.Read())
                {
                    index++;
                    string name = getAllProductReader.GetString(1);
                    name = name.Trim();
                    double quantity = getAllProductReader.GetDouble(2);
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

                    var createWarehouseIfNotExistResponse = db.CreateIfNotExists(new Warehouse()
                    {
                        Name = "Kho 1",
                        Address = "Châu Đức, BRVT",
                        ShopId = command.ShopId.Value,
                    }, e => new { e.ShopId, e.Name, e.Address });

                    CreateIfNotExistResponse<Product> createIfNotExistResponse = db.CreateIfNotExists(new Product()
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
                        ProductAssets = new ProductAsset[] {
                             new ProductAsset
                            {

                                AssetType =  ProductAssetConstants.ThumbnailAssetType,
                               Asset = productAsset
                            }
                        },
                        WarehouseProducts = new WarehouseProduct[]{
                            new WarehouseProduct(){
                                WarehouseId = createWarehouseIfNotExistResponse.Entity.Id,
                                Quantity = Math.Max(0,quantity),
                            }
                        },
                    }, e => new { e.ShopId, e.Name });
                    products.Add(createIfNotExistResponse.Entity);

                }
                #endregion

                #region SeedCustomers
                if (!environment.IsDevelopment())
                {
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
                        if (!string.IsNullOrEmpty(name))
                        {
                            dbCustomers.Add(new DbCustomerModel(oldId, name, address, currentDebt, debtHistories));
                        }

                    }
                    foreach (DbCustomerModel dbCustomer in dbCustomers)
                    {


                        CreateIfNotExistResponse<Customer> createIfNotExistResponse = db.CreateIfNotExists(new Customer
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
                }
                #endregion



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
            return Task.FromResult(responseResultBuilder.Build());
        }

    }
}