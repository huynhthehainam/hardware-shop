using HardwareShop.Core.Bases;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.Core.Services
{
    public interface IResponseResultBuilder
    {
        IActionResult Build();
        void SetUpdatedMessage();
        void SetData(object? data);
        void SetDeletedMessage();
        void SetMessage(IDictionary<SupportedLanguage, string> message);
        void SetNoContent();
        void SetFile(byte[] bytes, string contentType, string fileName);

        void SetCreatedObject<T>(T entity) where T : EntityBase;
        void AddInvalidFieldError(string fieldName);
        void AddExistedEntityError(string entityName);
        void AddNotFoundEntityError(string entityName);
        void AddNotPermittedError();
        void SetPageData<T>(PageData<T> pageData);
    }

}
