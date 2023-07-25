using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareShop.Business.Extensions
{
    public enum ApplicationErrorType
    {
        Invalid,
        NotPermitted,
        NotFound,
        Existed,
    }
    public class ApplicationError
    {
        public ApplicationErrorType Type { get; set; }
        public string? Message { get; set; }
        public ApplicationError(ApplicationErrorType type, string? message)
        {
            this.Type = type;
            this.Message = message;
        }
        public static ApplicationError CreateInvalidError(string message)
        {
            return new(ApplicationErrorType.Invalid, message);
        }
        public static ApplicationError CreateNotFoundError(string msg)
        {
            return new(ApplicationErrorType.NotFound, msg);
        }
        public static ApplicationError CreateExistedError(string msg)
        {
            return new(ApplicationErrorType.Existed, msg);
        }
        public static ApplicationError CreateNotPermittedError(string msg)
        {
            return new(ApplicationErrorType.NotPermitted, msg);
        }
    }
    public class ApplicationResponse<T>
    {
        public ApplicationError? Error { get; set; }
        public T? Result { get; set; }
    }
    public static class ApplicationResponseExtensions
    {
        public static void SetApplicationResponse<T>(this IResponseResultBuilder responseResultBuilder, ApplicationResponse<T> response, Action<IResponseResultBuilder, T> onSuccess)
        {
            if (response.Error != null)
            {
                switch (response.Error.Type)
                {
                    case ApplicationErrorType.Invalid:
                        responseResultBuilder.AddInvalidFieldError(response.Error.Message ?? "");
                        break;
                    case ApplicationErrorType.NotFound:
                        responseResultBuilder.AddNotFoundEntityError(response.Error.Message ?? "");
                        break;
                    case ApplicationErrorType.NotPermitted:
                        responseResultBuilder.AddNotPermittedError();
                        break;
                    case ApplicationErrorType.Existed:
                        responseResultBuilder.AddExistedEntityError(response.Error.Message ?? "");
                        break;
                }
            }
            else
            {
                if (response.Result == null)
                {
                    throw new Exception("Result must not be null");
                }
                onSuccess(responseResultBuilder, response.Result);
            }
        }
    }
    public static class BusinessExtensions
    {
        public static void ConfigureBusiness(this IServiceCollection services)
        {
            services.ConfigureRepository();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICustomerDebtService, CustomerDebtService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUnitCategoryService, UnitCategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IChatService, ChatService>();
        }
    }
}
